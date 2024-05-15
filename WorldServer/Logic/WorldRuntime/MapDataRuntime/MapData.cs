using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapData
	{
		public MapData(Int32 mapId, TerrainInfo terrainInfo)
		{
			MapId = mapId;
			TerrainInfo = terrainInfo;
		}

		public int MapId { get; private set; }
		public TerrainInfo TerrainInfo { get; private set; }
	}
}
