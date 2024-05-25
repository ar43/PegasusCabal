using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapData
	{
		public MapData(Int32 mapId, TerrainInfo terrainInfo, Dictionary<Int32, NpcData> npcData, Dictionary<int, MobSpawnData> mobSpawnData)
		{
			MapId = mapId;
			TerrainInfo = terrainInfo;
			NpcData = npcData;
			MobSpawnData = mobSpawnData;
		}

		public int MapId { get; private set; }
		public TerrainInfo TerrainInfo { get; private set; }
		public Dictionary<int, NpcData> NpcData { get; private set; }
		public Dictionary<int, MobSpawnData> MobSpawnData { get; private set; }
	}
}
