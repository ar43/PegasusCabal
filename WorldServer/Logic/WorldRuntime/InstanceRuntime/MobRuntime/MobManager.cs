namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime
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

		public Mob GetMob(int id)
		{
			if(_mobs.TryGetValue(id, out Mob? mob))
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
			var rng = new Random();
			DateTime time = DateTime.UtcNow;
			foreach (var mSpawn in _instance.MapData.MobSpawnData.Values)
            {
                var mobId = GetNextMobId();
                Mob mob = new Mob(mSpawn.MobData, mSpawn, _instance, mobId, rng);
                _mobs.Add(mobId, mob);
                mob.Spawn(time);
            }
        }

		public void UpdateAll()
		{
			if (_instance.NumClients == 0)
				return;

			DateTime time = DateTime.UtcNow;
			foreach(var mob in _mobs.Values)
			{
				mob.Update(time);
			}
		}
    }
}
