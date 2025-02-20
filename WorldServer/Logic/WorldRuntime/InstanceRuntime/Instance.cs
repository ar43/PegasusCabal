﻿using LibPegasus.Packets;
using LibPegasus.Parsers.Mcl;
using LibPegasus.Utils;
using System.Net.Sockets;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.Delegates;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MissionDungeonRuntime;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime;
using WorldServer.Packets.C2S.PacketSpecificData;
using WorldServer.Packets.S2C;
using WorldServer.Packets.S2C.PacketSpecificData;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime
{
	internal class Instance
	{
		public Instance(UInt16 mapId, InstanceDuration durationType, MapData mapData, InstanceType type)
		{
			MapId = (MapId)mapId;
			_cells = new Cell[NUM_CELL_X, NUM_CELL_Y];
			MapData = mapData;
			DurationType = durationType;
			NumClients = 0;
			MarkForDeletion = false;
			_lastInactiveTime = DateTime.UtcNow;
			Type = type;

			for (int i = 0; i < _cells.GetLength(0); i++)
			{
				for (int j = 0; j < _cells.GetLength(1); j++)
				{
					_cells[i, j] = new Cell();
				}
			}

			if (durationType == InstanceDuration.PERMANENT)
			{
				Id = (UInt64)MapId;
			}
			else
			{
				Id = InstanceIdGenerator;
				InstanceIdGenerator++;
			}
			var rngSeed = Guid.NewGuid().GetHashCode() + (int)Id + (int)DateTime.UtcNow.Ticks;
			//Serilog.Log.Debug(rngSeed.ToString());
			Rng = new Random(rngSeed);
		}

		public Instance(MapId mapId, InstanceDuration durationType, MapData mapData, InstanceType type) : this((UInt16)mapId, durationType, mapData, type) { }

		private readonly Cell[,] _cells;
		public readonly MapData MapData;
		public MobManager? MobManager = null;
		public GroundItemManager? GroundItemManager = null;
		public MissionDungeonManager? MissionDungeonManager = null;
		public Random Rng { get; private set; }

		public TileAttributeData? TileAttributeData { get; set; }
		public UInt64 Id { get; }
		public MapId MapId { get; }
		public InstanceDuration DurationType { get; }
		public InstanceType Type { get; }

		private static UInt64 InstanceIdGenerator = 1000;
		public static double INSTANCE_KILL_TIME = 500;

		public static readonly int NUM_CELL_X = 16;
		public static readonly int NUM_CELL_Y = 16;

		public bool MarkForDeletion { get; private set;}
		DateTime _lastInactiveTime;

		public int NumClients { get; private set; }

		public void Broadcast(PacketS2C packet)
		{
			foreach (var cell in _cells)
			{
				foreach(var client in cell.LocalClients)
				{
					client.PacketManager.Send(packet);
				}
			}
		}

		public void NotifyAllDungeonEnd(byte success, DungeonEndCause cause)
		{
			foreach (var cell in _cells)
			{
				foreach (var client in cell.LocalClients)
				{
					var nfy = new NFY_QuestDungeonEnd(client.Character.Id, success, cause);
					client.PacketManager.Send(nfy);

					client.Character.QuestManager.OnDungeonCompleted(MissionDungeonManager.GetDungeonId());
				}
			}
		}

		public void AddNewClient(Client client, UInt16 cellX, UInt16 cellY)
		{
			NumClients++;
			_cells[cellX, cellY].LocalClients.Add(client);
		}

		public void AddMobToCell(Mob mob, UInt16 cellX, UInt16 cellY, bool notifyAround)
		{
			_cells[cellX, cellY].LocalMobs.Add(mob);
			if (notifyAround)
			{
				//possibly optimize this... no need to broadcast to empty instance for example
				BroadcastNearby(mob, new NFY_NewMobsList(new List<Mob> { mob }));
			}
		}

		public void RemoveMobFromCell(Mob mob, bool notifyAround, DelObjectType type = DelObjectType.WARP)
		{
			_cells[mob.Movement.CellX, mob.Movement.CellY].LocalMobs.Remove(mob);
			if (notifyAround)
			{
				//possibly optimize this... no need to broadcast to empty instance for example
				BroadcastNearby(mob, new NFY_DelMobsList(mob.ObjectIndexData, type));
			}
		}

		public void AddGroundItemToCell(GroundItem groundItem, UInt16 cellX, UInt16 cellY, bool notifyAround)
		{
			_cells[cellX, cellY].LocalGroundItems.Add(groundItem);
			if (notifyAround)
			{
				//possibly optimize this... no need to broadcast to empty instance for example
				BroadcastNearby(groundItem, new NFY_NewItemList(new List<GroundItem> { groundItem }, (Byte)MapId));
			}
		}

		public void RemoveGroundItemFromCell(GroundItem groundItem, bool notifyAround, DelObjectType type = DelObjectType.DISAPPEAR)
		{
			_cells[groundItem.CellX, groundItem.CellY].LocalGroundItems.Remove(groundItem);
			if (notifyAround)
			{
				//possibly optimize this... no need to broadcast to empty instance for example
				BroadcastNearby(groundItem, new NFY_DelItemList(groundItem.ObjectIndexData, type));
			}
		}

		public List<(int, int)> CalculateValidCellSpots(UInt16 cellX, UInt16 cellY, bool mustBeTown)
		{
			List<(int, int)> values = new List<(int, int)>();
			var baseX = cellX * 16;
			var baseY = cellY * 16;

			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					if (!CheckTileMoveDisable((UInt16)(baseX + i), (UInt16)(baseY + j)))
					{
						if (!mustBeTown || CheckTileTown((UInt16)(baseX + i), (UInt16)(baseY + j)))
							values.Add((baseX + i, baseY + j));
					}
				}
			}
			return values;
		}

		public void RemoveClient(Client client, DelObjectType reason)
		{
			var packet_del = new NFY_DelUserList(client.Character.Id, reason);
			BroadcastNearby(client, packet_del, false);

			foreach (var cell in _cells)
			{
				if (cell.LocalClients.Remove(client))
				{
					client.Character.Location.Instance = null;
					NumClients--;
					if(NumClients == 0)
						_lastInactiveTime = DateTime.UtcNow;
					if (NumClients < 0)
						throw new Exception("negative NumClients");
					return;
				}
			}

			throw new Exception("not expected");
		}
		public bool CheckTileMoveDisable(UInt16 x, UInt16 y)
		{
			if (x > 255 || y > 255)
				return true;
			return TileAttributeData.HasTileAttribute(x, y, LibPegasus.Enums.TileAttribute.MASK_MOVEDSABLE);
		}
		public bool CheckTileOverAttack(UInt16 x, UInt16 y)
		{
			if (x > 255 || y > 255)
				return true;
			return TileAttributeData.HasTileAttribute(x, y, LibPegasus.Enums.TileAttribute.MASK_OVERATTCK0);
		}
		public bool CheckTileMobsLayer(UInt16 x, UInt16 y)
		{
			if (x > 255 || y > 255)
				return false;
			return TileAttributeData.IsTileMobsLayer(x, y);
		}
		public bool CheckTileTown(UInt16 x, UInt16 y)
		{
			if (x > 255 || y > 255)
				return false;
			return TileAttributeData.HasTileAttribute(x, y, LibPegasus.Enums.TileAttribute.MASK_TOWNAREAS0);
		}
		public void MoveMob(Mob mob, UInt16 newCellX, UInt16 newCellY)
		{
			var cellX = mob.Movement.CellX; //old cell pos x
			var cellY = mob.Movement.CellY; //old cell pos y

			if (Math.Abs(cellX - newCellX) > 1 || Math.Abs(cellY - newCellY) > 1)
				throw new Exception("incorrent MoveMob");

			var currentCell = _cells[cellX, cellY];
			var newCell = _cells[newCellX, newCellY];

			//TODO verify difference between current and new Cell

			var currentNeighbours = GetNeighbours((Int16)cellX, (Int16)cellY);
			var newNeightbours = GetNeighbours((Int16)newCellX, (Int16)newCellY);

			var relevantCells = newNeightbours.Except(currentNeighbours).ToList();

			foreach (var cellPos in relevantCells)
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c.Character == null)
						throw new Exception("Character should not be null here");

					var packetNewUser = new NFY_NewMobsList(new List<Mob>() { mob });
					c.PacketManager.Send(packetNewUser);
				}
			}

			currentCell.LocalMobs.Remove(mob);
			newCell.LocalMobs.Add(mob);

			mob.Movement.UpdateCellPos();
		}
		public void MoveClient(Client client, UInt16 newCellX, UInt16 newCellY, AddObjectType cellMoveType)
		{
			var cellX = client.Character.Location.Movement.CellX; //old cell pos x
			var cellY = client.Character.Location.Movement.CellY; //old cell pos y

			if (Math.Abs(cellX - newCellX) > 1 || Math.Abs(cellY - newCellY) > 1)
				throw new Exception("incorrent MoveClient");

			var currentCell = _cells[cellX, cellY];
			var newCell = _cells[newCellX, newCellY];

			//TODO verify difference between current and new Cell

			var currentNeighbours = GetNeighbours((Int16)cellX, (Int16)cellY);
			var newNeightbours = GetNeighbours((Int16)newCellX, (Int16)newCellY);
			var relevantCells = newNeightbours.Except(currentNeighbours).ToList();

			//TODO: this is not efficient
			var currentNeighboursShort = GetNeighboursShort((Int16)cellX, (Int16)cellY);
			var newNeightboursShort = GetNeighboursShort((Int16)newCellX, (Int16)newCellY);
			var relevantCellsShort = newNeightboursShort.Except(currentNeighboursShort).ToList();

			List<Character> newChars = new List<Character>();
			List<Mob> newMobs = new List<Mob>();
			List<GroundItem> newItems = new List<GroundItem>();

			foreach (var cellPos in relevantCells)
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == client)
						continue;
					if (c.Character == null)
						throw new Exception("Character should not be null here");
					if (client.Character == null)
						throw new Exception("Character should not be null here");

					newChars.Add(c.Character);
					var packetNewUser = new NFY_NewUserList(new List<Character>() { client.Character }, cellMoveType);
					c.PacketManager.Send(packetNewUser);
				}

				foreach (var m in _cells[cellPos.Item1, cellPos.Item2].LocalMobs)
				{
					if (m == null)
						throw new NullReferenceException("null mob");
					if (m.IsSpawned == false)
						throw new Exception("mob not spawned, shouldnt be in a cell");

					newMobs.Add(m);
				}
			}

			//TODO: this is not efficient, two loops
			foreach (var cellPos in relevantCellsShort)
			{
				foreach (var i in _cells[cellPos.Item1, cellPos.Item2].LocalGroundItems)
				{
					if (i == null)
						throw new NullReferenceException("null item");
					if (i.Active == false)
						throw new Exception("item not active, shouldnt be in a cell");

					newItems.Add(i);
				}
			}

			if (newChars.Count > 0)
			{
				var packetOtherPlayers = new NFY_NewUserList(newChars, AddObjectType.OTHERPLAYERS);
				client.PacketManager.Send(packetOtherPlayers);
			}

			if (newMobs.Count > 0)
			{
				var packetNewMobs = new NFY_NewMobsList(newMobs);
				client.PacketManager.Send(packetNewMobs);
			}

			if (newItems.Count > 0)
			{
				var packetNewItems = new NFY_NewItemList(newItems, 0, 0xFFFFFFFF);
				client.PacketManager.Send(packetNewItems);
			}

			currentCell.LocalClients.Remove(client);
			newCell.LocalClients.Add(client);

			client.Character.Location.Movement.UpdateCellPos();
		}
		private static List<(Int16, Int16)> GetNeighbours(Int16 cellX, Int16 cellY)
		{
			List<(Int16, Int16)> values = new List<(Int16, Int16)>();

			Int16[,] offsets = { { 0, 0 }, { 1, 0 }, { 2, 0 }, { -1, 0 }, { -2, 0 },
										   { 0, 1 }, { 0, 2 }, { 0, -1 }, { 0, -2 },
								{ 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };

			for (int i = 0; i < 13; i++)
			{
				var x = cellX + offsets[i, 0];
				var y = cellY + offsets[i, 1];
				if (x >= 0 && y >= 0 && x < NUM_CELL_X && y < NUM_CELL_Y)
				{
					values.Add(((Int16, Int16))(x, y));
				}
			}
			return values;
		}
		private static List<(Int16, Int16)> GetNeighboursShort(Int16 cellX, Int16 cellY)
		{
			List<(Int16, Int16)> values = new List<(Int16, Int16)>();

			Int16[,] offsets = { { 0, 0 }, { 1, 0 }, { -1, 0 },
										   { 0, 1 }, { 0, -1 },
								{ 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };

			for (int i = 0; i < 9; i++)
			{
				var x = cellX + offsets[i, 0];
				var y = cellY + offsets[i, 1];
				if (x >= 0 && y >= 0 && x < NUM_CELL_X && y < NUM_CELL_Y)
				{
					values.Add(((Int16, Int16))(x, y));
				}
			}
			return values;
		}

		public void BroadcastNearby(Client client, PacketS2C packet, bool excludeClient)
		{
			var cellX = client.Character.Location.Movement.CellX;
			var cellY = client.Character.Location.Movement.CellY;

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == null)
						throw new NullReferenceException("null client");
					if (c.Character == null)
						throw new NullReferenceException("null client");
					if (c == client && excludeClient)
						continue;

					c.PacketManager.Send(packet);
				}
			}
		}
		public void BroadcastNearby(Mob mob, PacketS2C packet)
		{
			var cellX = mob.Movement.CellX;
			var cellY = mob.Movement.CellY;

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == null)
						throw new NullReferenceException("null client");
					if (c.Character == null)
						throw new NullReferenceException("null client");

					c.PacketManager.Send(packet);
				}
			}
		}
		public void BroadcastNearby(GroundItem groundItem, PacketS2C packet)
		{
			var cellX = groundItem.CellX;
			var cellY = groundItem.CellY;

			foreach (var cellPos in GetNeighboursShort((Int16)cellX, (Int16)cellY))
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == null)
						throw new NullReferenceException("null client");
					if (c.Character == null)
						throw new NullReferenceException("null client");

					c.PacketManager.Send(packet);
				}
			}
		}
		public List<Character> GetNearbyCharacters(Client client)
		{
			List<Character> characters = new List<Character>();

			var cellX = client.Character.Location.Movement.CellX;
			var cellY = client.Character.Location.Movement.CellY;

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == client)
						continue;

					if (c.Character == null)
						throw new NullReferenceException("null character");

					characters.Add(c.Character);
				}
			}
			return characters;
		}

		public List<Mob> GetNearbyMobs(Client client)
		{
			List<Mob> mobs = new List<Mob>();

			var cellX = client.Character.Location.Movement.CellX;
			var cellY = client.Character.Location.Movement.CellY;

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var m in _cells[cellPos.Item1, cellPos.Item2].LocalMobs)
				{
					if (m == null)
						throw new NullReferenceException("null mob");
					if (m.IsSpawned == false)
						throw new Exception("mob not spawned, shouldnt be in a cell");

					mobs.Add(m);
				}
			}
			return mobs;
		}

		public List<GroundItem> GetNearbyGroundItems(Client client)
		{
			List<GroundItem> groundItems = new List<GroundItem>();

			var cellX = client.Character.Location.Movement.CellX;
			var cellY = client.Character.Location.Movement.CellY;

			foreach (var cellPos in GetNeighboursShort((Int16)cellX, (Int16)cellY))
			{
				foreach (var i in _cells[cellPos.Item1, cellPos.Item2].LocalGroundItems)
				{
					if (i == null)
						throw new NullReferenceException("null mob");
					if (i.Active == false)
						throw new Exception("item inactive, shouldnt be in a cell");

					groundItems.Add(i);
				}
			}
			return groundItems;
		}

		internal void Update()
		{
			if(NumClients == 0 && DurationType == InstanceDuration.TEMP)
			{
				DateTime time = DateTime.UtcNow;
				var diff = time - _lastInactiveTime;
				if (diff.TotalSeconds > INSTANCE_KILL_TIME)
					MarkForDeletion = true;
			}

			MobManager.UpdateAll();
			MissionDungeonManager?.Update();
		}

		internal bool OnUserSkillAttacksMob(Client attacker, List<MobTarget> defenders, int x, int y, int skillSlot)
		{
			bool combo = false;
			if (defenders.Count == 0)
			{
				Serilog.Log.Error("empty defenders");
				return false;
			}
			var defenderTargetInfo = defenders[0]; //TODO

			Mob defenderTarget;
			try
			{
				defenderTarget = MobManager.GetMob(defenderTargetInfo.Id.ObjectId);
			}
			catch
			{
				Serilog.Log.Error("cant find defender");
				return false;
			}

			if (defenderTarget == null)
			{
				Serilog.Log.Error("defender is null");
				return false;
			}

			if (defenderTarget.IsDead)
			{
				Serilog.Log.Warning("defender is dead");
				return false;
			}

			if (attacker.Character.Location.Movement.IsMoving)
			{
				Serilog.Log.Warning("attacker is moving");
				return false;
			}

			if (CheckTileTown((UInt16)attacker.Character.Location.Movement.X, (UInt16)attacker.Character.Location.Movement.Y))
			{
				Serilog.Log.Warning("attacker is in town");
				return false;
			}

			if (defenders.Count > 255)
			{
				//hard cap, but there should be an even lower cap, TODO, also kick the client
				Serilog.Log.Warning("abnormal mob list");
				return false;
			}

			//todo, cooldown check 
			//todo, position check

			var skill = attacker.Character.Skills.Get(skillSlot);
			if (skill == null)
				return false;

			var battleStats = attacker.Character.CalculateBattleStats();
			var skillAttackRate = skill.CalculateAttackRate(battleStats.AttackRate);
			var skillAttack = skill.CalculateAttack(battleStats.Attack, battleStats.SwordSkillAmp, battleStats.MagicAttack, battleStats.MagicSkillAmp);
			var skillCriticalRate = skill.CalculateCritRate(battleStats.CriticalRate, battleStats.MaxCriticalRate);
			var skillCriticalDamage = skill.CalculateCritDamage(battleStats.CriticalDamage);
			var mobDmgReport = new List<DamageToMobResult>();

			int skillExp = 0;

			foreach (var defenderInfo in defenders)
			{
				var defender = MobManager.GetMob(defenderInfo.Id.ObjectId);
				var lvlDiffOrg = (int)(attacker.Character.Stats.Level - defender.Level);
				var lvlDiff = BattleFormula.GetLvlDiff(attacker.Character.Stats.Level, defender.Level);
				int roll = Rng.Next(100);

				int damage = 0;
				AttackResult attackResult;
				int exp = 0;

				//TODO: check if defender is in range of defenderTarget / attacker

				if (roll < skillCriticalRate)
				{
					Serilog.Log.Debug($"CRIT calculated skillAttack: {skillAttack} roll: {roll} final CR: {skillCriticalRate} finalCD: {skillCriticalDamage}");
					damage = defender.CalculateCriticalDamageTaken(attacker.Character, skill, skillAttack, skillCriticalDamage);
					attackResult = AttackResult.SR_CRITICAL;
				}
				else
				{
					skillAttackRate = (Int32)(skillAttackRate + 16 * lvlDiffOrg / 10);
					if (skillAttackRate < 0)
						skillAttackRate = 0;

					var defenderDefenseRate = defender.GetDefenseRate();
					int hitRate = 0;

					if (skillAttackRate + defenderDefenseRate == 0)
					{
						hitRate = 0;
					}
					else
					{
						hitRate = BattleFormula.GetHR(skillAttackRate, defenderDefenseRate);
					}

					hitRate = BattleFormula.AdjustHR(hitRate, 30, 95 - defender.GetEvasion());
					hitRate = (100 - skillCriticalRate) * hitRate / 100 + skillCriticalRate;
					Serilog.Log.Debug($"calculated skillAttack: {skillAttack} hitRate: {hitRate} roll: {roll} final CR: {skillCriticalRate}");

					if (combo || roll < hitRate)
					{
						damage = defender.CalculateNormalDamageTaken(attacker.Character, skill, skillAttack);
						attackResult = AttackResult.SR_NORMALAK;
					}
					else
					{
						attackResult = AttackResult.SR_MISSINGS;
					}
				}

				if (attackResult != AttackResult.SR_MISSINGS)
				{
					if (damage < 1)
						damage = 1;
				}

				if (damage > 0)
				{
					//TODO: absorb
				}

				if (attackResult != AttackResult.SR_MISSINGS)
				{
					int expModifier = attacker.Character.RestExpCheck();
					//TODO: exp
					if (defender.GetMaxHP() == 0)
					{
						exp = 0;
						skillExp = 0;
					}
					else
					{
						exp = BattleFormula.GetEXP(damage, defender.GetMaxHP(), defender.GetExp(), lvlDiff, 1); //this is 25% too much cuz of EP8 settings

						if (attackResult == AttackResult.SR_CRITICAL)
						{
							skillExp = skill.GetSkillExp(2);
						}
						else
						{
							skillExp = skill.GetSkillExp(1);
						}
					}
				}

				exp = (400 - 600 / (1 + 1)) * exp / 100 / 1; //what is this? some kind of scrapped aoe diminishing returns for exp??

				defender.TakeDamage(damage);
				defender.AggroTable.Add(damage, attacker, defender.CurrentDefender);
				defender.SetAttacker(attacker);


				var dmgResult = new DamageToMobResult(defender.ObjectIndexData);
				dmgResult.HasBFX = 1;
				dmgResult.DamageReceived = (UInt16)damage;
				dmgResult.AttackResult = attackResult;
				dmgResult.Type = defenderInfo.Type; //what is this TODO
				dmgResult.HPLeft = (UInt32)defender.HP;
				mobDmgReport.Add(dmgResult);

				defender.DeathCheck(attacker, skill.Id);

				if(exp > 1)
				{
					//TODO: recalc Exp for bonuses and blah blah
					attacker.Character.Stats.AddExp((UInt64)exp);
				}
			}

			
			//also skill exp

			
			//addSkillExp

			var rsp = new RSP_SkillToMobs(mobDmgReport, skill.Id, (UInt16)attacker.Character.Status.Hp, (UInt16)attacker.Character.Status.Mp,
				(UInt16)attacker.Character.Status.Sp, attacker.Character.Stats.Exp, (UInt32)skillExp, 0, 0, 0);
			attacker.PacketManager.Send(rsp);

			var nfy = new NFY_SkillToMobs(mobDmgReport, skill.Id, attacker.Character.Id, (UInt16)x, (UInt16)y);
			BroadcastNearby(attacker, nfy, false);

			return true;
		}
	}
}
