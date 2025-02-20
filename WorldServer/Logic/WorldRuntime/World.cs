﻿using Grpc.Net.Client.Configuration;
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
	}
}
