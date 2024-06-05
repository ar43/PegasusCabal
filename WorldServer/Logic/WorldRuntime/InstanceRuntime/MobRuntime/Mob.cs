using LibPegasus.Utils;
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
        public byte Level { get; private set; }
        public byte Nation { get; private set; }
        public bool IsSpawned { get; private set; } = false;
        public bool IsChasing { get; private set; } = false;
        private List<(int, int)>? _spawnSpots = null;
		private DateTime _nextUpdateTime;
		private int _findCounter;
		private UInt16 _spawnPosX;
		private UInt16 _spawnPosY;
		private UInt16 _chasePosX;
		private UInt16 _chasePosY;
		private Random _rng;
		private MobPhase _phase;

		private readonly int SPAWN_DELAY = 2000;

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
			ResetFindCounter();
			if(first)
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

			SetNextUpdateTime(currentTime, _data.MoveInterval);
		}

		private void NextFindPhase(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.FIND);

			SetNextUpdateTime(currentTime, _data.FindInterval);
			_findCounter--;
		}

		public int GetMaxHP()
        {
            return _data.HP;
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
                    if (!_instance.CheckTerrainCollision((UInt16)(baseX + i), (UInt16)(baseY + j)) && !_instance.CheckTileTown((UInt16)(baseX + i), (UInt16)(baseY + j)))
                    {
                        values.Add((baseX + i, baseY + j));
                    }
                }
            }
            return values;
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
            _instance.AddMobToCell(this, (UInt16)Movement.CellX, (UInt16)Movement.CellY, true);

			SetPhaseFind(currentTime, true);

			Level = (Byte)(_data.LEV + _rng.Next(3));
            HP = GetMaxHP();
			IsSpawned = true;
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

			if (_instance.CheckTerrainCollision((UInt16)endX, (UInt16)endY))
				return false;

			if (_instance.CheckTileTown((UInt16)endX, (UInt16)endY))
				return false;

			if (startX == endX && startY == endY)
				return false;

			if(Movement.Begin((UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY, (UInt16)pntX, (UInt16)pntY))
			{
				var packet = new NFY_MobsMoveBgn(ObjectIndexData, (UInt32)Movement.StartTime, (UInt16)startX, (UInt16)startY, (UInt16)endX, (UInt16)endY);
				_instance.BroadcastNearby(this, packet);
			}
			else
			{
				throw new Exception("idk");
			}

			return true;
		}

		private bool IsTooFarFromSpawn()
		{
			//todo, return false if in quest dungeon
			if(GetDistance(_spawnPosX, _spawnPosY, Movement.X, Movement.Y) > _data.Limt1Range)
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
			int iDx = x2 - x1,
									iDy = y2 - y1;


			if (iDx < 0) iDx = -iDx;
			if (iDy < 0) iDy = -iDy;

			return (iDx >= iDy) ? iDx : iDy;
		}

		private void Find(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.FIND);

			if (_findCounter <= 1)
			{
				SetPhaseMove(currentTime);
				return;
			}

			NextFindPhase(currentTime);
		}

		private void Move(DateTime currentTime)
		{
			Debug.Assert(_phase == MobPhase.MOVE);

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

		public int TakeNormalDamage(Character attacker, Skill skill, int attack)
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

			return iDamage;
		}
		
		public void Update(DateTime currentTime)
		{
			if (currentTime < _nextUpdateTime)
				return;

			if (!IsSpawned)
				return;

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
				default:
				{
					throw new Exception("Invalid phase");
				}
			}
		}
    }
}
