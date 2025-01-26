using Grpc.Net.Client.Configuration;
using System.Globalization;
using WorldServer.Logic.WorldRuntime.InstanceRuntime;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;
using WorldServer.Logic.WorldRuntime.ShopRuntime;
using WorldServer.Logic.WorldRuntime.WarpsRuntime;

namespace WorldServer.Logic.WorldRuntime
{
	internal class World
	{
		private WorldConfig _worldConfig;
		private WarpManager _warpManager;
		private MapDataManager _mapDataManager;
		public ShopPoolManager ShopPoolManager { get; private set; }
		private MobDataManager _mobDataManager;
		private MissionDungeonDataManager _missionDungeonDataManager;

		public InstanceManager InstanceManager { get; private set; }

		//mapId, dungeonId, speciesId
		public static Dictionary<(int, int, int), List<WorldDropData>>? WorldDropTable { get; private set; }

		public World()
		{
			_worldConfig = new WorldConfig();
			ShopPoolManager = new ShopPoolManager(_worldConfig);
			_mobDataManager = new MobDataManager(_worldConfig);
			_warpManager = new WarpManager(_worldConfig);
			_mapDataManager = new MapDataManager(_worldConfig, _mobDataManager);
			_missionDungeonDataManager = new MissionDungeonDataManager(_worldConfig, _mobDataManager);
			InstanceManager = new InstanceManager(_worldConfig, _warpManager, _mapDataManager, _missionDungeonDataManager);
		}

		internal void Update()
		{
			InstanceManager.Update();
		}

		public static void LoadConfig(WorldConfig worldConfig)
		{
			if (WorldDropTable != null) throw new Exception("WorldDrop configs already loaded");
			WorldDropTable = new();

			var cfg = worldConfig.GetConfig("[WorldDrop]");

			foreach(var worldDrop in cfg.Values)
			{
				int Terrain_World = Convert.ToInt32(worldDrop["Terrain_World"]);
				int DungeonID = Convert.ToInt32(worldDrop["DungeonID"]);
				int Terrain_Mob = Convert.ToInt32(worldDrop["Terrain_Mob"]);
				int ItemKind = Convert.ToInt32(worldDrop["ItemKind"]);
				int ItemOpt = Convert.ToInt32(worldDrop["ItemOpt"]);
				int DropRate = (int)((Convert.ToDouble(worldDrop["DropRate"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0) * Int32.MaxValue);
				if (DropRate <= 0)
					throw new Exception("unexpected DropRate");
				int MinLv = Convert.ToInt32(worldDrop["MinLv"]);
				int MaxLv = Convert.ToInt32(worldDrop["MaxLv"]);
				int Group = Convert.ToInt32(worldDrop["Group"]);
				int MaxDropCnt = Convert.ToInt32(worldDrop["MaxDropCnt"]);
				int OptPoolIdx = Convert.ToInt32(worldDrop["OptPoolIdx"]);
				int DurationIdx = Convert.ToInt32(worldDrop["DurationIdx"]);
				int DropSvrCh = Convert.ToInt32(worldDrop["DropSvrCh"]);
				int EventDropOnly = Convert.ToInt32(worldDrop["EventDropOnly"]);

				if (!WorldDropTable.ContainsKey((Terrain_World, DungeonID, Terrain_Mob)))
					WorldDropTable[(Terrain_World, DungeonID, Terrain_Mob)] = new();
				WorldDropTable[(Terrain_World, DungeonID, Terrain_Mob)].Add(new WorldDropData(Terrain_World, DungeonID, Terrain_Mob, ItemKind, ItemOpt, DropRate, MinLv, MaxLv, Group, MaxDropCnt, OptPoolIdx, DurationIdx, DropSvrCh, EventDropOnly));
			}
		}
	}
}
