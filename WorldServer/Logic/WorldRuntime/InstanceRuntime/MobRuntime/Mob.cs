using LibPegasus.Utils;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using System.Diagnostics;
using System.Security.Cryptography;
using WorldServer.Enums.Mob;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.SharedData;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;
using WorldServer.Packets.S2C;
using WorldServer.Packets.S2C.PacketSpecificData;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime
{
	internal class Mob
	{
		private MobData _data;
		private MobSpawnData _spawnData;
		private readonly Instance _instance;
		public ObjectIndexData ObjectIndexData { get; private set; }
		public MobMovementData Movement { get; private set; }
		public int HP { get; private set; } //temp, will be replaced by some Status class
		public int Level { get; private set; }
		public byte Nation { get; private set; }
		public bool IsSpawned { get; private set; } = false;
		public bool IsDead { get; private set; } = false;
		public bool IsChasing { get; private set; } = false;
		public int AddedAlertRange { get; private set; }
		private List<(int, int)>? _spawnSpots = null;
		private DateTime _nextUpdateTime;
		private int _findCounter;
		private int _spawnPosX;
		private int _spawnPosY;
		private int _chasePosX;
		private int _chasePosY;
		private Random _rng;
		private MobPhase _phase;

		private const int MAX_REACTION_RANGE = 16;

		public AggroTable AggroTable { get; private set; }
		public Client? LastAttacker { get; private set; }
		public Client? CurrentDefender { get; private set; }
		public bool IsAttacked { get; private set; }

		public MobSkill CurrentSkill { get; private set; }


		private int evasion = 0;

		private readonly int SPAWN_DELAY = 2000;

		private int HP30Breakpoint;
		private int HP50Breakpoint;

		public Mob(MobData data, MobSpawnData spawnData, Instance instance, UInt16 id, Random rng)
		{
			_data = data;
			_spawnData = spawnData;
			_instance = instance;
			ObjectIndexData = new ObjectIndexData(id, (Byte)_instance.MapId, Enums.ObjectType.MOB); //why is world index this? todo: figure out
			Nation = 0;
			Movement = new(0, 0, _data.MoveSpeed);
			ResetFindCounter();
			_nextUpdateTime = DateTime.MinValue;
			_rng = rng;
			_phase = MobPhase.INVALID;
			AggroTable = new AggroTable();
			LastAttacker = null;
			CurrentDefender = null;
			IsAttacked = false;
			CurrentSkill = _data.DefaultSkill;

			if (_instance.Type == Enums.InstanceType.FIELD)
				AddedAlertRange = 2;
			else
				AddedAlertRange = 32;

		}

		private void SetNextUpdateTime(DateTime currentTime, int ms)
		{
			_nextUpdateTime = currentTime;
			_nextUpdateTime = _nextUpdateTime.AddTicks(TimeSpan.FromMilliseconds(ms).Ticks);
		}

		private void ResetFindCounter()
		{
			_findCounter = _data.FindCount;
		}

		private void SetPhaseFind(DateTime currentTime, bool first = false)
		{
			_phase = MobPhase.FIND;
			IsChasing = false;
			ResetFindCounter();
			if (first)
			{
				SetNextUpdateTime(currentTime, _data.FindInterval + SPAWN_DELAY);
			}
			else
			{
				SetNextUpdateTime(currentTime, _data.FindInterval);
			}
		}

		private void SetPhaseMove(DateTime currentTime)
		{
			_phase = MobPhase.MOVE;
			IsChasing = false;
			SetNextUpdateTime(currentTime, _data.MoveInterval);
		}

		private void NextFindPhase(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.FIND);

			SetNextUpdateTime(currentTime, _data.FindInterval);
			_findCounter--;
		}

		private void NextBattlePhase(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.BATTLE);

			SetNextUpdateTime(currentTime, _data.ChasInterval);
		}

		private void NextChasePhase(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.CHASE);

			SetNextUpdateTime(currentTime, _data.ChasInterval);
		}

		public int GetMaxHP()
		{
			return _data.HP;
		}

		public int GetExp()
		{
			return _data.EXP;
		}

		public int GetDefense()
		{
			return _data.Defense;
		}

		public UInt16 GetSpecies()
		{
			if (_spawnData.SpeciesIdx != _data.Id)
				throw new Exception("Fatal mob data error");
			return (UInt16)_spawnData.SpeciesIdx;
		}

		private List<(int, int)> CalculateValidSpawnSpots(UInt16 baseX, UInt16 baseY, int width, int height)
		{
			List<(int, int)> values = new List<(int, int)>();

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (!_instance.CheckTileMoveDisable((UInt16)(baseX + i), (UInt16)(baseY + j)) && !_instance.CheckTileTown((UInt16)(baseX + i), (UInt16)(baseY + j)))
					{
						values.Add((baseX + i, baseY + j));
					}
				}
			}
			return values;
		}

		public void SetAttacker(Client? client)
		{
			if (client == null)
			{
				LastAttacker = null;
				IsAttacked = false;
			}
			else
			{
				LastAttacker = client;
				IsAttacked = true;
			}
		}


		public void Spawn(DateTime currentTime)
		{
			if (_spawnSpots == null)
				_spawnSpots = CalculateValidSpawnSpots((UInt16)_spawnData.PosX, (UInt16)_spawnData.PosY, _spawnData.Width, _spawnData.Height);
			if (_spawnSpots.Count <= 0)
				throw new Exception("No valid spawn spots");

			var position = _spawnSpots[_rng.Next(_spawnSpots.Count)];
			var newX = position.Item1;
			var newY = position.Item2;

			if (IsSpawned)
				_instance.RemoveMobFromCell(this, true, Enums.DelObjectType.WARP);

			Movement = new(0, 0, _data.MoveSpeed);
			Movement.SetPosition(newX, newY);
			Movement.UpdateCellPos();
			_spawnPosX = (UInt16)newX;
			_spawnPosY = (UInt16)newY;
			_chasePosX = (UInt16)newX;
			_chasePosY = (UInt16)newY;
			IsSpawned = true;
			IsDead = false;
			SetPhaseFind(currentTime, true);
			UnselectTarget();
			SetAttacker(null);

			HP30Breakpoint = GetMaxHP() * 3 / 10;
			HP50Breakpoint = GetMaxHP() * 5 / 10;

			Level = (Byte)(_data.LEV + _rng.Next(3));
			HP = GetMaxHP();

			_instance.AddMobToCell(this, (UInt16)Movement.CellX, (UInt16)Movement.CellY, true);
		}

		private bool GenerateMovement()
		{
			var startX = Movement.X;
			var startY = Movement.Y;
			var endX = _rng.Next(8) + _chasePosX - 4;
			var endY = _rng.Next(8) + _chasePosY - 4;
			var pntX = endX;
			var pntY = endY;

			if (endX < 0 || endY < 0 || endX > 255 || endY > 255)
				return false;

			if (_instance.CheckTileMoveDisable((UInt16)endX, (UInt16)endY))
				return false;

			if (_instance.CheckTileTown((UInt16)endX, (UInt16)endY))
				return false;

			if (startX == endX && startY == endY)
				return false;

			if (Movement.Begin((UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY, (UInt16)pntX, (UInt16)pntY))
			{
				var packet = new NFY_MobsMoveBgn(ObjectIndexData, (UInt32)Movement.StartTime, (UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY);
				_instance.BroadcastNearby(this, packet);
			}
			else
			{
				throw new Exception("should not occur");
			}

			return true;
		}

		private bool IsTooFarFromSpawn()
		{
			//todo, return false if in quest dungeon
			if (GetDistance(_spawnPosX, _spawnPosY, Movement.X, Movement.Y) > _data.Limt1Range)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static int GetDistance(int x1, int y1, int x2, int y2)
		{
			int iDx = x2 - x1, iDy = y2 - y1;


			if (iDx < 0) iDx = -iDx;
			if (iDy < 0) iDy = -iDy;

			return (iDx >= iDy) ? iDx : iDy;
		}

		private void VerifyFindPhase()
		{
			if (CurrentDefender != null)
			{
				throw new Exception("CurrentDefender != null");
			}

			if (IsChasing)
			{
				throw new Exception("IsChasing");
			}

			if (Movement.IsMoving)
			{
				throw new Exception("IsMoving");
			}

			if (Movement.IsDeadReckoning)
			{
				throw new Exception("IsDeadReckoning");
			}

			if (Movement.EndX != Movement.StartX)
			{
				throw new Exception("Movement mismatch");
			}

			if (Movement.EndY != Movement.StartY)
			{
				throw new Exception("Movement mismatch");
			}

			if (Movement.X != Movement.StartX)
			{
				throw new Exception("Movement mismatch");
			}

			if (Movement.Y != Movement.StartY)
			{
				throw new Exception("Movement mismatch");
			}
		}

		private void UnselectTarget()
		{
			CurrentDefender = null;
			AggroTable.Reset();
		}

		private void SwitchAttackerToDefender()
		{
			CurrentDefender = LastAttacker;
			LastAttacker = null;
		}

		private void SetPhaseBattle(DateTime currentTime)
		{
			_phase = MobPhase.BATTLE;
			Movement.MoveSpeed = _data.ChasSpeed;
			IsChasing = true;

			NextBattlePhase(currentTime);
		}

		private void SetPhaseChase(DateTime currentTime)
		{
			_phase = MobPhase.CHASE;
			Movement.MoveSpeed = _data.ChasSpeed;
			IsChasing = true;

			NextChasePhase(currentTime);
		}

		private void FindWhileAttacked(DateTime currentTime)
		{
			//this is meant to only run once after being attacked in find phase, all or nothing
			Debug.Assert(IsAttacked == true);

			NextFindPhase(currentTime);
			IsAttacked = false;

			if (LastAttacker == null || LastAttacker.Dropped || LastAttacker.Character.Location.Instance == null || LastAttacker.Character.Location.Instance.Id != _instance.Id)
			{
				if (!Movement.IsMoving)
				{
					_chasePosX = Movement.X;
					_chasePosY = Movement.Y;
				}
				UnselectTarget();
				if (!Movement.IsMoving)
				{
					SetPhaseFind(currentTime);
				}
				return;
			}

			//todo: quest dungeon cell attr check
			//todo: another quest dungeon check

			var attackerMovement = LastAttacker.Character.Location.Movement;

			if (!_instance.CheckTileMobsLayer((UInt16)attackerMovement.X, (UInt16)attackerMovement.Y))
			{
				return;
			}

			var distance = GetDistance(Movement.X, Movement.Y, attackerMovement.X, attackerMovement.Y);

			if (distance < MAX_REACTION_RANGE)
			{
				int targetPos = attackerMovement.X | (attackerMovement.Y << 16);
				if (distance <= CurrentSkill.ValidDist && MobSkill.IsValid(Movement.X, Movement.Y, targetPos, _instance))
				{
					SwitchAttackerToDefender();
					SetPhaseBattle(currentTime);
				}
				else if (!_data.IsMoveless)
				{
					SwitchAttackerToDefender();

					var endX = attackerMovement.X;
					var endY = attackerMovement.Y;
					var startX = (UInt16)Movement.StartX;
					var startY = (UInt16)Movement.StartY;

					//todo pathfinding....

					//todo AdjustTargetPoint...

					if (Movement.X == endX && Movement.Y == endY)
					{
						//when does this happen
						_chasePosX = Movement.StartX;
						_chasePosY = Movement.StartY;
						UnselectTarget();
						SetPhaseMove(currentTime);
						return;
					}

					if (Movement.Begin(startX, startY, (UInt16)endX, (UInt16)endY, (UInt16)endX, (UInt16)endY))
					{
						SetPhaseChase(currentTime);
						var packet = new NFY_MobsChasBgn(ObjectIndexData, (UInt32)Movement.StartTime, (UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY);
						_instance.BroadcastNearby(this, packet);
					}
					else
					{
						throw new Exception("idk");
					}

				}

			}
		}

		private void FindNeutral(DateTime currentTime)
		{
			if (_findCounter <= 1)
			{
				SetPhaseMove(currentTime);
				return;
			}

			NextFindPhase(currentTime);
		}

		private void Find(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.FIND);
			VerifyFindPhase();

			if (IsAttacked)
			{
				FindWhileAttacked(currentTime);
			}
			else
			{
				//todo: aggressive
				FindNeutral(currentTime);
			}
		}

		private void MoveAttacked(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.MOVE);
			Debug.Assert(IsAttacked == true);
			Debug.Assert(IsChasing == false);
			Debug.Assert(CurrentDefender == null);
			Debug.Assert(LastAttacker != null);
			Debug.Assert(!_data.IsMoveless);
			bool reqMoveEnd = false;

			SetPhaseMove(currentTime);

			if (IsTooFarFromSpawn())
			{
				Spawn(currentTime);
				return;
			}

			IsAttacked = false;
			if (Movement.IsMoving)
			{
				if (Movement.IsDeadReckoning)
				{
					var oldX = Movement.X;
					var oldY = Movement.Y;
					Movement.DeadReckoning();

					var newCellX = Movement.X / 16;
					var newCellY = Movement.Y / 16;
					if (Movement.CellX != newCellX || Movement.CellY != newCellY)
					{
						//change cell
						_instance.MoveMob(this, (UInt16)newCellX, (UInt16)newCellY);
					}
				}

				if (!Movement.IsDeadReckoning)
				{
					//Serilog.Log.Debug($"no longer dead reckoning {Movement.X} {Movement.Y} {Movement.EndX} {Movement.EndY}");
					if (Movement.End((UInt16)Movement.X, (UInt16)Movement.Y))
					{
						var packet = new NFY_MobsMoveEnd(ObjectIndexData, (UInt16)Movement.EndX, (UInt16)Movement.EndY);
						_instance.BroadcastNearby(this, packet);
						reqMoveEnd = true;
					}
					else
					{
						throw new Exception("idk");
					}
				}
			}

			if (LastAttacker == null || LastAttacker.Dropped || LastAttacker.Character.Location.Instance == null || LastAttacker.Character.Location.Instance.Id != _instance.Id)
			{
				LastAttacker = null;
				return;
			}

			//QD related things todo

			var attackerMovement = LastAttacker.Character.Location.Movement;

			if (!_instance.CheckTileMobsLayer((UInt16)attackerMovement.X, (UInt16)attackerMovement.Y))
			{
				return;
			}

			var distance = GetDistance(Movement.X, Movement.Y, attackerMovement.X, attackerMovement.Y);

			if (distance <= MAX_REACTION_RANGE)
			{
				if (!reqMoveEnd)
				{
					Movement.ForceEndMovement();
				}

				int targetPos = attackerMovement.X | (attackerMovement.Y << 16);
				if (distance <= CurrentSkill.ValidDist && MobSkill.IsValid(Movement.X, Movement.Y, targetPos, _instance))
				{
					SwitchAttackerToDefender();
					SetPhaseBattle(currentTime);
				}
				else if (!_data.IsMoveless)
				{
					SwitchAttackerToDefender();

					var endX = attackerMovement.X;
					var endY = attackerMovement.Y;
					var startX = (UInt16)Movement.StartX;
					var startY = (UInt16)Movement.StartY;

					//todo pathfinding....

					//todo AdjustTargetPoint...

					if (Movement.X == endX && Movement.Y == endY)
					{
						//when does this happen
						_chasePosX = Movement.StartX;
						_chasePosY = Movement.StartY;
						UnselectTarget();
						SetPhaseMove(currentTime);
						return;
					}

					if (Movement.Begin(startX, startY, (UInt16)endX, (UInt16)endY, (UInt16)endX, (UInt16)endY))
					{
						SetPhaseChase(currentTime);
						var packet = new NFY_MobsChasBgn(ObjectIndexData, (UInt32)Movement.StartTime, (UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY);
						_instance.BroadcastNearby(this, packet);
					}
					else
					{
						throw new Exception("idk");
					}

				}
			}

		}

		private void MoveNeutral(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.MOVE);
			Debug.Assert(IsAttacked == false);
			Movement.MoveSpeed = _data.MoveSpeed;

			if (Movement.IsMoving)
			{
				if (Movement.IsDeadReckoning)
				{
					var oldX = Movement.X;
					var oldY = Movement.Y;
					Movement.DeadReckoning();

					if (IsTooFarFromSpawn())
					{
						Spawn(currentTime);
						return;
					}

					var newCellX = Movement.X / 16;
					var newCellY = Movement.Y / 16;
					if (Movement.CellX != newCellX || Movement.CellY != newCellY)
					{
						//change cell
						_instance.MoveMob(this, (UInt16)newCellX, (UInt16)newCellY);
					}
					if (Movement.IsDeadReckoning)
					{
						SetPhaseMove(currentTime);
						return;
					}
				}

				if (!Movement.IsDeadReckoning)
				{
					//Serilog.Log.Debug($"no longer dead reckoning {Movement.X} {Movement.Y} {Movement.EndX} {Movement.EndY}");
					if (Movement.End((UInt16)Movement.X, (UInt16)Movement.Y))
					{
						var packet = new NFY_MobsMoveEnd(ObjectIndexData, (UInt16)Movement.EndX, (UInt16)Movement.EndY);
						_instance.BroadcastNearby(this, packet);
						SetPhaseFind(currentTime);
						return;
					}
					else
					{
						throw new Exception("idk");
					}
				}
				SetNextUpdateTime(currentTime, _data.MoveInterval);
				return;
			}
			else
			{
				if (!GenerateMovement())
				{
					SetPhaseFind(currentTime);
					return;
				}

				SetPhaseMove(currentTime);
				return;
			}
		}

		private void AssertStill()
		{
			Debug.Assert(Movement.StartX == Movement.EndX);
			Debug.Assert(Movement.StartY == Movement.EndY);
			Debug.Assert(Movement.X == Movement.StartX);
			Debug.Assert(Movement.Y == Movement.StartY);
		}

		private void BattleAttacked(DateTime currentTime)
		{
			NextBattlePhaseBySkill(currentTime);

			Debug.Assert(IsChasing && CurrentDefender != null);

			IsAttacked = false;

			CurrentDefender = AggroTable.MaxAggroChar;

			Debug.Assert(CurrentDefender != null);

			if (OpponentInvalid(CurrentDefender, currentTime))
				return;

			var defenderMovement = CurrentDefender.Character.Location.Movement;

			if (CurrentDefender.Character.Status.IsDead || !_instance.CheckTileMobsLayer((UInt16)defenderMovement.X, (UInt16)defenderMovement.Y))
			{
				_chasePosX = Movement.X;
				_chasePosY = Movement.Y;
				UnselectTarget();
				SetPhaseFind(currentTime);
				return;
			}

			var distance = GetDistance(Movement.X, Movement.Y, defenderMovement.X, defenderMovement.Y);

			if (distance > _data.AlertRange + AddedAlertRange)
			{
				_chasePosX = Movement.X;
				_chasePosY = Movement.Y;
				UnselectTarget();
				SetPhaseFind(currentTime);
				return;
			}
			else
			{
				switch(_data.AttkPattern)
				{
					case MobPattern.PATTERN1:
					{
						if(distance > CurrentSkill.ValidDist)
						{
							SetPhaseChase(currentTime);
							return;
						}
						else if(HP <= HP30Breakpoint)
						{
							CurrentSkill = _data.SpecialSkill;
							HP30Breakpoint = 0;
						}
						break;
					}
					case MobPattern.PATTERN2:
					{
						Debug.Assert(CurrentSkill == _data.DefaultSkill);
						if(LastAttacker != null && LastAttacker != CurrentDefender)
						{
							//TODO
							throw new NotImplementedException("mob pattern 2");
						}
						break;
					}
					case MobPattern.PATTERN3:
					{
						Debug.Assert(_data.DefaultSkill.Reach >= _data.SpecialSkill.Reach);

						if (distance > CurrentSkill.ValidDist)
						{
							CurrentSkill = _data.DefaultSkill;
							if (distance > _data.DefaultSkill.Reach)
							{
								SetPhaseChase(currentTime);
								return;
							}
						}
						else if (_data.SpecialSkill.Reach > distance)
						{
							CurrentSkill = _data.SpecialSkill;
						}
						break;
					}
					case MobPattern.PATTERN4:
					{
						SetRandomSkill();

						if (distance > CurrentSkill.ValidDist)
						{
							SetPhaseChase(currentTime);
							return;
						}
						break;
					}
					default:
					{
						if(distance > CurrentSkill.ValidDist)
						{
							SetPhaseChase(currentTime);
							return;
						}
						break;
					}
				}
			}

			Debug.Assert(CurrentSkill.SkillGroup == Enums.SkillGroup.SK_GROUP___);

			MobAttackG001(CurrentDefender, CurrentSkill, currentTime);
		}

		private bool OpponentInvalid(Client opponent, DateTime currentTime)
		{
			if (opponent == null || opponent.Dropped || opponent.Character.Location.Instance == null || opponent.Character.Location.Instance.Id != _instance.Id)
			{
				if (!Movement.IsMoving)
				{
					_chasePosX = Movement.X;
					_chasePosY = Movement.Y;
				}
				UnselectTarget();
				if (!Movement.IsMoving)
				{
					SetPhaseFind(currentTime);
				}
				return true;
			}
			return false;
		}

		private void SetRandomSkill()
		{
			var val = _rng.Next(10);

			if(val > 5)
			{
				CurrentSkill = _data.DefaultSkill;
			}
			else
			{
				CurrentSkill = _data.SpecialSkill;
			}
		}

		private void BattleNeutral(DateTime currentTime) 
		{
			NextBattlePhaseBySkill(currentTime);

			Debug.Assert(IsChasing);
			Debug.Assert(CurrentDefender != null);

			if (OpponentInvalid(CurrentDefender, currentTime))
				return;

			var defenderMovement = CurrentDefender.Character.Location.Movement;

			if(CurrentDefender.Character.Status.IsDead || !_instance.CheckTileMobsLayer((UInt16)defenderMovement.X, (UInt16)defenderMovement.Y))
			{
				_chasePosX = Movement.X;
				_chasePosY = Movement.Y;
				UnselectTarget();
				SetPhaseFind(currentTime);
				return;
			}

			var distance = GetDistance(Movement.X, Movement.Y, defenderMovement.X, defenderMovement.Y);

			if (distance > _data.AlertRange + AddedAlertRange)
			{
				_chasePosX = Movement.X;
				_chasePosY = Movement.Y;
				UnselectTarget();
				SetPhaseFind(currentTime);
				return;
			}
			else
			{
				if (_data.AttkPattern == MobPattern.PATTERN3)
				{
					Debug.Assert(_data.DefaultSkill.Reach >= _data.SpecialSkill.Reach);

					if (distance > CurrentSkill.ValidDist)
					{
						CurrentSkill = _data.DefaultSkill;
						if (distance > _data.DefaultSkill.Reach)
						{
							SetPhaseChase(currentTime);
							return;
						}
					}
					else if(_data.SpecialSkill.Reach > distance)
					{
						CurrentSkill = _data.SpecialSkill;
					}

				}
				else if (_data.AttkPattern == MobPattern.PATTERN4)
				{
					SetRandomSkill();

					if (distance > CurrentSkill.ValidDist)
					{
						SetPhaseChase(currentTime);
						return;
					}
				}
				else if(distance > CurrentSkill.ValidDist)
				{
					SetPhaseChase(currentTime);
					return;
				}
			}

			Debug.Assert(CurrentSkill.SkillGroup == Enums.SkillGroup.SK_GROUP___);

			MobAttackG001(CurrentDefender, CurrentSkill, currentTime);
		}

		private void NextBattlePhaseBySkill(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.BATTLE);

			SetNextUpdateTime(currentTime, CurrentSkill.Interval);
		}

		private void MobAttackG001(Client defender, MobSkill skill, DateTime currentTime)
		{
			Debug.Assert(skill.SkillGroup == Enums.SkillGroup.SK_GROUP___);
			var defenderStatus = defender.Character.Status;

			if(defenderStatus.IsDead)
			{
				Debug.Assert(defenderStatus.Hp <= 0);
				return;
			}

			var defenderMovement = defender.Character.Location.Movement;

			var targetPos = defenderMovement.X | (defenderMovement.Y << 16);

			if(!MobSkill.IsValid(Movement.X, Movement.Y, targetPos, _instance))
			{
				if(_data.IsMoveless || defender.Dropped)
				{
					UnselectTarget();
					SetPhaseFind(currentTime);
				}
				else
				{
					Debug.Assert(false);
					//PhaseAttackValid??? Gate?????
					SetPhaseChase(currentTime);
				}
				return;
			}

			var res = ResolveAttack(defender, skill);

			var takeDamageRes = defender.Character.TakeDamage(res.Damage);
			res.RemainingHp = defender.Character.Status.Hp;
			res.IsDead = takeDamageRes == Enums.TakeDamageResult.DEAD;

			var nfy = new NFY_SkillByMWide(ObjectIndexData, skill.IsDefSkill, [res]);
			_instance.BroadcastNearby(this, nfy);

		}

		

		private void Battle(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.BATTLE);
			if(IsAttacked)
			{
				BattleAttacked(currentTime);
			}
			else
			{
				BattleNeutral(currentTime);
			}
		}

		private void ChaseNeutral(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.CHASE);
			Debug.Assert(!IsAttacked);

			NextChasePhase(currentTime);

			Debug.Assert(IsChasing && CurrentDefender != null);
			Debug.Assert(!_data.IsMoveless);

			if (IsTooFarFromSpawn())
			{
				Spawn(currentTime);
				return;
			}

			if (!Movement.IsMoving)
			{
				Debug.Assert(!Movement.IsDeadReckoning);
				AssertStill();

				CurrentDefender = AggroTable.MaxAggroChar;

				if (CurrentDefender == null || CurrentDefender.Dropped || CurrentDefender.Character.Location.Instance == null || CurrentDefender.Character.Location.Instance.Id != _instance.Id)
				{
					if (!Movement.IsMoving)
					{
						_chasePosX = Movement.X;
						_chasePosY = Movement.Y;
					}
					UnselectTarget();
					if (!Movement.IsMoving)
					{
						SetPhaseFind(currentTime);
					}
					return;
				}

				var defenderMovement = CurrentDefender.Character.Location.Movement;

				if (!_instance.CheckTileMobsLayer((UInt16)defenderMovement.X, (UInt16)defenderMovement.Y))
				{
					_chasePosX = Movement.X;
					_chasePosY = Movement.Y;
					var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.X, (UInt16)Movement.Y);
					_instance.BroadcastNearby(this, packet);
					UnselectTarget();
					SetPhaseFind(currentTime);
					return;
				}

				//TODO MASK_MOBLAYERS??
				//TODO QD

				var distance = GetDistance(Movement.X, Movement.Y, defenderMovement.X, defenderMovement.Y);
				if (distance > _data.AlertRange + AddedAlertRange)
				{
					_chasePosX = Movement.X;
					_chasePosY = Movement.Y;
					UnselectTarget();

					var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.X, (UInt16)Movement.Y);
					_instance.BroadcastNearby(this, packet);

					SetPhaseFind(currentTime);
				}
				else
				{
					int targetPos = defenderMovement.X | (defenderMovement.Y << 16);
					if (distance <= CurrentSkill.ValidDist && MobSkill.IsValid(Movement.X, Movement.Y, targetPos, _instance))
					{
						SetPhaseBattle(currentTime);
					}
					else if (!_data.IsMoveless)
					{

						var endX = defenderMovement.X;
						var endY = defenderMovement.Y;
						var startX = (UInt16)Movement.StartX;
						var startY = (UInt16)Movement.StartY;

						//todo pathfinding....

						//todo AdjustTargetPoint...

						if (Movement.X == endX && Movement.Y == endY)
						{
							//when does this happen
							_chasePosX = Movement.StartX;
							_chasePosY = Movement.StartY;
							UnselectTarget();
							var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.X, (UInt16)Movement.Y);
							_instance.BroadcastNearby(this, packet);
							SetPhaseMove(currentTime);
							return;
						}

						if (Movement.Begin(startX, startY, (UInt16)endX, (UInt16)endY, (UInt16)endX, (UInt16)endY))
						{
							SetPhaseChase(currentTime);
							var packet = new NFY_MobsChasBgn(ObjectIndexData, (UInt32)Movement.StartTime, (UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY);
							_instance.BroadcastNearby(this, packet);
						}
						else
						{
							throw new Exception("idk");
						}

					}
				}
			}
			else
			{
				Debug.Assert(Movement.IsDeadReckoning);
				Debug.Assert(Movement.StartX != Movement.EndX || Movement.StartY != Movement.EndY);
				Movement.MoveSpeed = _data.ChasSpeed;

				if (Movement.IsDeadReckoning)
				{
					var oldX = Movement.X;
					var oldY = Movement.Y;
					Movement.DeadReckoning();

					var newCellX = Movement.X / 16;
					var newCellY = Movement.Y / 16;
					if (Movement.CellX != newCellX || Movement.CellY != newCellY)
					{
						//change cell
						_instance.MoveMob(this, (UInt16)newCellX, (UInt16)newCellY);
					}
				}

				if (!Movement.IsDeadReckoning)
				{
					//Serilog.Log.Debug($"no longer dead reckoning {Movement.X} {Movement.Y} {Movement.EndX} {Movement.EndY}");
					if (Movement.End((UInt16)Movement.X, (UInt16)Movement.Y))
					{
						var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.EndX, (UInt16)Movement.EndY);
						_instance.BroadcastNearby(this, packet);
					}
					else
					{
						throw new Exception("idk");
					}
				}
			}
		}

		private void ChaseAttacked(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.CHASE);
			Debug.Assert(IsAttacked);

			NextChasePhase(currentTime);
			Debug.Assert(IsChasing && CurrentDefender != null);
			Debug.Assert(IsAttacked && LastAttacker != null);
			Debug.Assert(!_data.IsMoveless);

			if (IsTooFarFromSpawn())
			{
				Spawn(currentTime);
				return;
			}

			IsAttacked = false;
			if(!Movement.IsMoving)
			{
				Debug.Assert(!Movement.IsDeadReckoning);
				AssertStill();

				CurrentDefender = AggroTable.MaxAggroChar;

				if (CurrentDefender == null || CurrentDefender.Dropped || CurrentDefender.Character.Location.Instance == null || CurrentDefender.Character.Location.Instance.Id != _instance.Id)
				{
					if (!Movement.IsMoving)
					{
						_chasePosX = Movement.X;
						_chasePosY = Movement.Y;
					}
					UnselectTarget();
					if (!Movement.IsMoving)
					{
						SetPhaseFind(currentTime);
					}
					return;
				}

				var defenderMovement = CurrentDefender.Character.Location.Movement;

				if (!_instance.CheckTileMobsLayer((UInt16)defenderMovement.X, (UInt16)defenderMovement.Y))
				{
					_chasePosX = Movement.X;
					_chasePosY = Movement.Y;
					var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.X, (UInt16)Movement.Y);
					_instance.BroadcastNearby(this, packet);
					UnselectTarget();
					SetPhaseFind(currentTime);
					return;
				}

				//TODO QD

				var distance = GetDistance(Movement.X, Movement.Y, defenderMovement.X, defenderMovement.Y);
				if(distance > _data.AlertRange + AddedAlertRange)
				{
					_chasePosX = Movement.X;
					_chasePosY = Movement.Y;
					UnselectTarget();

					var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.X, (UInt16)Movement.Y);
					_instance.BroadcastNearby(this, packet);

					SetPhaseFind(currentTime);
				}
				else
				{
					int targetPos = defenderMovement.X | (defenderMovement.Y << 16);
					if (distance <= CurrentSkill.ValidDist && MobSkill.IsValid(Movement.X, Movement.Y, targetPos, _instance))
					{
						SetPhaseBattle(currentTime);
					}
					else if (!_data.IsMoveless)
					{

						var endX = defenderMovement.X;
						var endY = defenderMovement.Y;
						var startX = (UInt16)Movement.StartX;
						var startY = (UInt16)Movement.StartY;

						//todo pathfinding....

						//todo AdjustTargetPoint...

						if (Movement.X == endX && Movement.Y == endY)
						{
							//when does this happen
							_chasePosX = Movement.StartX;
							_chasePosY = Movement.StartY;
							UnselectTarget();
							var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.X, (UInt16)Movement.Y);
							_instance.BroadcastNearby(this, packet);
							SetPhaseMove(currentTime);
							return;
						}

						if (Movement.Begin(startX, startY, (UInt16)endX, (UInt16)endY, (UInt16)endX, (UInt16)endY))
						{
							SetPhaseChase(currentTime);
							var packet = new NFY_MobsChasBgn(ObjectIndexData, (UInt32)Movement.StartTime, (UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY);
							_instance.BroadcastNearby(this, packet);
						}
						else
						{
							throw new Exception("idk");
						}

					}
				}
			}
			else
			{
				Debug.Assert(Movement.IsDeadReckoning);
				Debug.Assert(Movement.StartX != Movement.EndX || Movement.StartY != Movement.EndY);
				Movement.MoveSpeed = _data.ChasSpeed;

				if (Movement.IsDeadReckoning)
				{
					var oldX = Movement.X;
					var oldY = Movement.Y;
					Movement.DeadReckoning();

					var newCellX = Movement.X / 16;
					var newCellY = Movement.Y / 16;
					if (Movement.CellX != newCellX || Movement.CellY != newCellY)
					{
						//change cell
						_instance.MoveMob(this, (UInt16)newCellX, (UInt16)newCellY);
					}
				}

				if (!Movement.IsDeadReckoning)
				{
					//Serilog.Log.Debug($"no longer dead reckoning {Movement.X} {Movement.Y} {Movement.EndX} {Movement.EndY}");
					if (Movement.End((UInt16)Movement.X, (UInt16)Movement.Y))
					{
						var packet = new NFY_MobsChasEnd(ObjectIndexData, (UInt16)Movement.EndX, (UInt16)Movement.EndY);
						_instance.BroadcastNearby(this, packet);
					}
					else
					{
						throw new Exception("idk");
					}
				}
			}
		}

		public DamageFromMobResult ResolveAttack(Client defender, MobSkill skill)
		{
			var res = new DamageFromMobResult();
			var roll = _rng.Next(100);
			var lvDiff = BattleFormula.GetLvlDiff(Level, defender.Character.Stats.Level);
			var lvDiffOrg = Level - defender.Character.Stats.Level;

			if (lvDiff < -1000)
				lvDiff = -1000;
			else if(lvDiff > 1000) 
				lvDiff = 1000;

			var criticalRate = 5; //TODO
			var criticalDamage = 0; //TODO

			var battleStats = defender.Character.CalculateBattleStats();

			var defence = battleStats.Defense;
			var defenceRate = battleStats.DefenseRate;
			var evasion = 0; //TODO

			if(roll < criticalRate)
			{
				//crit
				var damage = skill.PhyAttMax - defence;
				if(damage <= 0)
				{
					damage = Level / 10 + Level / 30;
				}
				if(damage <= 0)
				{
					damage = 1;
				}
				damage += lvDiffOrg;
				damage = damage + (criticalDamage >> 1);

				if(damage <= 0)
				{
					damage = Level / 10 + Level / 20;
				}
				if (damage <= 0)
				{
					damage = 5;
				}

				res.AttackResult = Enums.AttackResult.SR_CRITICAL;
				res.Damage = (UInt16)damage;
			}
			else
			{
				//normal
				var attackRateTemp = _data.AttacksR + 16 * lvDiffOrg / 10;
				if(attackRateTemp <= 0)
					attackRateTemp = 0;
				int hitRate = 0;
				if(attackRateTemp + defenceRate == 0)
				{
					hitRate = 0;
				}
				else
				{
					hitRate = (BattleFormula.GetHR(attackRateTemp, defenceRate) - 50) * 2;
				}
				hitRate = BattleFormula.AdjustHR(hitRate, 10, 95 - evasion);
				hitRate = (100 - criticalRate) * hitRate / 100 + criticalRate;


				if(roll < hitRate)
				{
					var attack = 0;
					if (skill.PhyAttDff == 0)
					{
						attack = skill.PhyAttMin;
					}
					else
					{
						attack = _rng.Next(skill.PhyAttDff) + skill.PhyAttMin;
					}
					var damage = attack - defence;
					damage += lvDiffOrg;

					if (damage <= 0)
					{
						damage = Level / 10;
					}
					if (damage <= 0)
					{
						damage = 1;
					}
					res.AttackResult = Enums.AttackResult.SR_NORMALAK;
					res.Damage = (UInt16)damage;
				}
				else
				{
					res.Damage = 0;
					res.AttackResult = Enums.AttackResult.SR_MISSINGS;
				}
			}

			//TODO absorb
			//TODO WAR
			//TODO DEATH
			res.CharId = defender.Character.Id;
			return res;
		}

		private void Chase(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.CHASE);

			if (IsAttacked)
			{
				ChaseAttacked(currentTime);
			}
			else
			{
				ChaseNeutral(currentTime);
			}
		}

		private void Move(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.MOVE);

			if(IsAttacked)
			{
				MoveAttacked(currentTime);
			}
			else
			{
				MoveNeutral(currentTime);
			}
		}

		public int GetDefenseRate()
		{
			return _data.DefenseR;
		}

		public int GetEvasion()
		{
			return evasion;
		}

		public int CalculateNormalDamageTaken(Character attacker, Skill skill, int attack)
		{
			int iLvDiffOrg = (Int32)(attacker.Stats.Level - Level);
			int defence = GetDefense();
			int iDamage = attack - defence;
			if (iDamage <= 0)
				iDamage = 0;
			iDamage += iLvDiffOrg;
			if (iDamage <= 0)
				iDamage = 0;
			if (skill.IsSwordSkill())
			{
				int iDamageMin = iDamage * attacker.Style.BattleStyle.MinDmgRate / 100;
				iDamage = _rng.Next(iDamage - iDamageMin) + iDamageMin;
			}


			//TODO
			/*
			if (pSkillData->_sgGroup == SK_GROUP004)
			{
				iDamage = (iDamage == 0) ? 0 : (int)(iDamage * pAttacker->fHitMultiDmg);
			}
			else
			{
				iDamage = (iDamage == 0) ? 0 : iDamage + pAttacker->iDemageP;
			}
			*/

			int modified = (Int32)(attacker.Stats.Level / 10);
			if (iDamage < modified)
			{
				iDamage = modified;
			}
			if (iDamage <= 0)
				iDamage = 0;

			// Combo Damage
			/*
			if (iCombSkill)
			{
				switch (iTiming)
				{
					case CBST_GOOD0:
						iDamage = iDamage * 110 / 100;
						break;
					case CBST_EXCEL:
						iDamage = iDamage * 120 / 100;
						break;
					default:
						break;
				}
			}
			*/

			return Math.Min(iDamage, HP);
		}
		
		public void Update(DateTime currentTime)
		{
			if (currentTime < _nextUpdateTime)
				return;

			if(!IsSpawned && IsDead)
			{
				Serilog.Log.Debug($"Spawning mob {ObjectIndexData.ObjectId}");
				Spawn(currentTime);
				return;
			}
			else if (!IsSpawned)
			{
				throw new Exception("Not supposed to happen");
			}

			switch(_phase)
			{
				case MobPhase.FIND:
				{
					Find(currentTime);
					break;
				}
				case MobPhase.MOVE:
				{
					Move(currentTime);
					break;
				}
				case MobPhase.CHASE:
				{
					Chase(currentTime);
					break;
				}
				case MobPhase.BATTLE:
				{
					Battle(currentTime);
					break;
				}
				default:
				{
					throw new Exception("Invalid phase");
				}
			}
		}

		internal Int32 CalculateCriticalDamageTaken(Character attacker, Skill skill, Int32 attack, Int32 criticalDamage)
		{
			int iLvDiffOrg = (Int32)(attacker.Stats.Level - Level);
			int defence = GetDefense();
			int iDamage = attack - defence;
			if (iDamage <= 0)
				iDamage = 1;
			iDamage += iLvDiffOrg;
			if (iDamage <= 0)
				iDamage = 1;
			iDamage = iDamage * criticalDamage / 100;

			//TODO
			/*
			if (pSkillData->_sgGroup == SK_GROUP004)
			{
				iDamage = (iDamage == 0) ? 0 : (int)(iDamage * pAttacker->fHitMultiDmg);
			}
			else
			{
				iDamage = (iDamage == 0) ? 0 : iDamage + pAttacker->iDemageP;
			}
			*/

			int modified = (Int32)(attacker.Stats.Level / 10) + (Int32)(attacker.Stats.Level / 20);
			if (iDamage < modified)
			{
				iDamage = modified;
			}
			if (iDamage <= 0)
				iDamage = 5;

			//some war stuff missing

			return Math.Min(iDamage, HP);
		}

		internal void TakeDamage(Int32 damage)
		{
			HP -= damage;
		}

		internal void DeathCheck()
		{
			if(HP <= 0)
			{
				IsSpawned = false;
				IsDead = true;
				_instance.RemoveMobFromCell(this, false);
				SetNextUpdateTime(DateTime.UtcNow, _spawnData.SpwnInterval);
			}
		}
	}
}
