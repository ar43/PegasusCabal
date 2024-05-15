using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapDataManager
	{
		Dictionary<int, MapData> _maps;
		WorldConfig _config;

		public MapDataManager(WorldConfig config)
		{
			_maps = new();
			_config = config;	
		}

		public MapData Get(int mapId)
		{
			if (_maps.TryGetValue(mapId, out var map))
			{
				return map;
			}
			else
			{
				var configStr = "[Terrain" + Convert.ToString(mapId) + "]";
				var subConfig = _config.Config[configStr];
				if(subConfig != null)
				{
					var terrainData = subConfig["0"];
					if(terrainData != null)
					{
						var terrainX = Convert.ToInt32(terrainData["TerrainX"]);
						var terrainY = Convert.ToInt32(terrainData["TerrainY"]);
						var warpIdxForDead = Convert.ToInt32(terrainData["WarpIdxForDead"]);
						var warpIdxForRetn = Convert.ToInt32(terrainData["WarpIdxForRetn"]);
						var warpIdxForLOut = Convert.ToInt32(terrainData["WarpIdxForLOut"]);
						var dmgMin = Convert.ToInt32(terrainData["DmgMin"]);
						var dmgMax = Convert.ToInt32(terrainData["DmgMax"]);
						var warControl = Convert.ToInt32(terrainData["WarControl"]);


						var terrainInfo = new TerrainInfo(terrainX, terrainY, warpIdxForDead, warpIdxForRetn, warpIdxForLOut, dmgMin, dmgMax, warControl);
						var mapData = new MapData(mapId, terrainInfo);
						if (!_maps.TryAdd(mapId, mapData))
							throw new Exception($"Map {mapId} already exists");
						return mapData;
					}
					else
					{
						throw new Exception(configStr + " does not exist");
					}
				}
				else
				{
					throw new Exception(configStr + " does not exist");
				}
			}
		}
	}
}
