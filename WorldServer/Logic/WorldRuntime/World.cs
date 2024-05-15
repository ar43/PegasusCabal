using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.WarpsRuntime;

namespace WorldServer.Logic.WorldRuntime
{
	internal class World
	{
		private WorldConfig _worldConfig;
		private WarpManager _warpManager;
		private MapDataManager _mapDataManager;

		public InstanceManager InstanceManager { get; private set; }

		public World()
		{
			_worldConfig = new WorldConfig();
			_warpManager = new WarpManager(_worldConfig);
			_mapDataManager = new MapDataManager(_worldConfig);
			InstanceManager = new InstanceManager(_worldConfig, _warpManager, _mapDataManager);
		}
	}
}
