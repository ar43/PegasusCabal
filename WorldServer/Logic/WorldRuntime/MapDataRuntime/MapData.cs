namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapData
	{
		public MapData(Int32 mapId, TerrainInfo terrainInfo, Dictionary<Int32, NpcData> npcData, Dictionary<int, MobSpawnData> mobSpawnData, Dictionary<(int, int, int), MissionDropData> localMissionDropData)
		{
			MapId = mapId;
			TerrainInfo = terrainInfo;
			NpcData = npcData;
			MobSpawnData = mobSpawnData;
			LocalMissionDropData = localMissionDropData;
		}

		public int MapId { get; private set; }
		public TerrainInfo TerrainInfo { get; private set; }
		public Dictionary<int, NpcData> NpcData { get; private set; }
		public Dictionary<int, MobSpawnData> MobSpawnData { get; private set; }
		public Dictionary<(int,int,int), MissionDropData> LocalMissionDropData { get; private set; }
	}
}
