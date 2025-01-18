using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime
{
	internal class MobManager
	{
		Dictionary<int, Mob> _mobs;
		private readonly Instance _instance;
		private UInt16 _mobIdGenerator = 0;

		public MobManager(Instance instance, bool addMapMobs)
		{
			_instance = instance;
			_mobs = new();

			if (addMapMobs)
				AddAllMobs();
		}

		private UInt16 GetNextMobId()
		{
			_mobIdGenerator++;
			return _mobIdGenerator;
		}

		internal void KillMob(int mobId, DelObjectType delObjectType = DelObjectType.DEAD, Client? attacker = null, int skillId = 0)
		{
			if (!_mobs.ContainsKey(mobId))
				throw new Exception("mob does not exist");
			_mobs[mobId].Kill(delObjectType, attacker, skillId);
		}

		internal void DeleteMob(int mobId, DelObjectType delObjectType = DelObjectType.DEAD, Client? attacker = null, int skillId = 0)
		{
			if (!_mobs.ContainsKey(mobId))
				throw new Exception("mob does not exist");
			_mobs[mobId].Delete(delObjectType, attacker, skillId);
		}

		public Mob GetMob(int id)
		{
			if (_mobs.TryGetValue(id, out Mob? mob))
			{
				return mob;
			}
			else
			{
				throw new Exception("Can't find mob");
			}
		}

		public void AddAllMobs()
		{
			Debug.Assert(_instance.Type == Enums.InstanceType.FIELD);
			var rng = new Random();
			DateTime time = DateTime.UtcNow;
			foreach (var mSpawn in _instance.MapData.MobSpawnData.Values)
			{
				var mobId = GetNextMobId();
				Mob mob = new Mob(mSpawn.MobData, mSpawn, _instance, mobId, rng, null);
				_mobs.Add(mobId, mob);
				mob.Spawn(time);
			}
		}

		public void SpawnDungeonMob(MissionDungeonMMapEntry spawnInfo)
		{
			Debug.Assert(_instance.Type == Enums.InstanceType.DUNGEON);

			ushort mobId = (UInt16)spawnInfo.ExtraMobInfo.MobIdx;
			Mob mob = new Mob(spawnInfo.MobSpawnData.MobData, spawnInfo.MobSpawnData, _instance, mobId, _instance.Rng, spawnInfo.ExtraMobInfo);

			if (_mobs.ContainsKey(mobId))
				throw new Exception("unexpected mobId");

			_mobs.Add(mobId, mob);
			mob.Spawn(DateTime.UtcNow);

		}

		public void UpdateAll()
		{
			if (_instance.NumClients == 0)
				return;

			DateTime time = DateTime.UtcNow;
			foreach (var mob in _mobs.Values)
			{
				mob.Update(time);
			}
		}
	}
}
