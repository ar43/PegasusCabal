using LibPegasus.Utils;
using Serilog;
using System.Security.Cryptography;
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
		private DateTime _lastMovementGen;

		private readonly int SPAWN_DELAY = 2000;

        public Mob(MobData data, MobSpawnData spawnData, Instance instance, UInt16 id)
        {
            _data = data;
            _spawnData = spawnData;
            _instance = instance;
            ObjectIndexData = new ObjectIndexData(id, (Byte)_instance.MapId, Enums.ObjectType.MOB); //why is world index this? todo: figure out
            Nation = 0;
			Movement = new(0, 0, _data.MoveSpeed);
			ResetFindCounter();
			_nextUpdateTime = DateTime.MinValue;
			_lastMovementGen = DateTime.MinValue;
		}

		private void SetNextUpdateTime(DateTime currentTime, int ms)
		{
			_nextUpdateTime = currentTime;
			_nextUpdateTime = _nextUpdateTime.AddTicks(TimeSpan.FromMilliseconds(ms).Ticks);
		}

		private void ResetFindCounter()
		{
			_findCounter = _data.FindCount - 1;
		}

        public int GetMaxHP()
        {
            return _data.HP;
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


        public void Spawn()
        {
            if (_spawnSpots == null)
                _spawnSpots = CalculateValidSpawnSpots((UInt16)_spawnData.PosX, (UInt16)_spawnData.PosY, _spawnData.Width, _spawnData.Height);
            if (_spawnSpots.Count <= 0)
                throw new Exception("No valid spawn spots");

            var position = _spawnSpots[RandomNumberGenerator.GetInt32(_spawnSpots.Count)];
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

			ResetFindCounter();
			SetNextUpdateTime(DateTime.UtcNow, _data.FindInterval + SPAWN_DELAY);

			Level = (Byte)(_data.LEV + RandomNumberGenerator.GetInt32(3));
            HP = GetMaxHP();
			IsSpawned = true;
		}

		private bool GenerateMovement()
		{
			var startX = Movement.X;
			var startY = Movement.Y;
			var endX = RandomNumberGenerator.GetInt32(8) + _chasePosX - 4;
			var endY = RandomNumberGenerator.GetInt32(8) + _chasePosY - 4;
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
				/*
				if(_lastMovementGen != DateTime.MinValue)
				{
					Serilog.Log.Debug($"Diff: {(DateTime.UtcNow - _lastMovementGen).TotalMilliseconds}");
				}
				_lastMovementGen = DateTime.UtcNow;
				*/
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

		public void Update(DateTime currentTime)
        {
			if (currentTime < _nextUpdateTime)
				return;

			if (!IsSpawned)
				return;

			if(Movement.IsMoving)
			{
				if(Movement.IsDeadReckoning)
				{
					var oldX = Movement.X;
					var oldY = Movement.Y;
					Movement.DeadReckoning();

					if(IsTooFarFromSpawn())
					{
						Spawn();
						return;
					}

					var newCellX = Movement.X / 16;
					var newCellY = Movement.Y / 16;
					if (Movement.CellX != newCellX || Movement.CellY != newCellY)
					{
						//change tiles
						_instance.MoveMob(this, (UInt16)newCellX, (UInt16)newCellY);
					}
					if(Movement.IsDeadReckoning)
					{
						SetNextUpdateTime(currentTime, _data.MoveInterval);
						return;
					}
				}

				if(!Movement.IsDeadReckoning)
				{
					//Serilog.Log.Debug($"no longer dead reckoning {Movement.X} {Movement.Y} {Movement.EndX} {Movement.EndY}");
					if(Movement.End((UInt16)Movement.X, (UInt16)Movement.Y))
					{
						var packet = new NFY_MobsMoveEnd(ObjectIndexData, (UInt16)Movement.EndX, (UInt16)Movement.EndY);
						_instance.BroadcastNearby(this, packet);
						ResetFindCounter();
						SetNextUpdateTime(currentTime, _data.FindInterval);
					}
					else
					{
						throw new Exception("idk");
					}
					return;
				}
				SetNextUpdateTime(currentTime, _data.MoveInterval);
				return;
			}

			if(_findCounter > 0)
			{
				_findCounter--;
				SetNextUpdateTime(currentTime, _data.FindInterval);
				return;
			}

			if(!Movement.IsMoving)
			{
				if(!GenerateMovement())
				{
					ResetFindCounter();
					SetNextUpdateTime(currentTime, _data.FindInterval);
					return;
				}

				SetNextUpdateTime(currentTime, _data.MoveInterval);
				return;
			}

			throw new NotImplementedException();
            
        }
    }
}
