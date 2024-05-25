using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.SharedData;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime
{
	internal class Mob
	{
		private MobData _data;
		private MobSpawnData _spawnData;
		private readonly Instance _instance;
		public ObjectIndexData ObjectIndexData { get; private set; }
		public int X { get; private set; } //temp, will be replaced with MobMovement
		public int Y { get; private set; } //temp, will be replaced with MobMovement
		public int HP { get; private set; } //temp, will be replaced by some Status class
		public byte Level { get; private set; }
		public byte Nation { get; private set; }
		public bool IsSpawned { get; private set; } = false;
		public bool IsChasing { get; private set; } = false;
		private List<(int, int)>? _spawnSpots = null;
		private Random _random = new Random();

		public Mob(MobData data, MobSpawnData spawnData, Instance instance, UInt16 id)
		{
			_data = data;
			_spawnData = spawnData;
			_instance = instance;
			ObjectIndexData = new ObjectIndexData(id, (Byte)_instance.MapId, Enums.ObjectType.MOB); //why is world index this? todo: figure out
			Nation = 0;
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
			IsSpawned = true;

			if (_spawnSpots == null)
				_spawnSpots = CalculateValidSpawnSpots((UInt16)_spawnData.PosX, (UInt16)_spawnData.PosY, _spawnData.Width, _spawnData.Height);
			if (_spawnSpots.Count <= 0)
				throw new Exception("No valid spawn spots");

			var position = _spawnSpots[_random.Next(_spawnSpots.Count)];
			var newX = position.Item1;
			var newY = position.Item2;

			X = newX; Y = newY;
			_instance.AddMobToCell(this, (UInt16)(X / 16), (UInt16)(Y / 16));

			Level = (Byte)(_data.LEV + (_random.Next() % 3));
			HP = GetMaxHP();
		}

		public void Update()
		{
			return;
		}
	}
}
