using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime
{
	internal class MobManager
	{
		Dictionary<int, Mob> _mobs;
		private readonly Instance _instance;
		private UInt16 _mobIdGenerator = 0;

		public MobManager(Instance instance)
		{
			_instance = instance;
			_mobs = new();

			AddAllMobs();
		}

		private UInt16 GetNextMobId()
		{
			_mobIdGenerator++;
			return _mobIdGenerator;
		}

		public void AddAllMobs()
		{
			foreach(var mSpawn in _instance.MapData.MobSpawnData.Values)
			{
				var mobId = GetNextMobId();
				Mob mob = new Mob(mSpawn.MobData, mSpawn, _instance, mobId);
				_mobs.Add(mobId, mob);
				mob.Spawn();
			}
		}
	}
}
