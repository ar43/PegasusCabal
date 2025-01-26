namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapData
	{
		public MapData(Int32 mapId, TerrainInfo terrainInfo, Dictionary<Int32, NpcData> npcData, Dictionary<Int32, MobSpawnData> mobSpawnData, Dictionary<(Int32, Int32, Int32), MissionDropData> localMissionDropData, Dictionary<Int32, List<LocalDropData>> localDropTable, Dictionary<Int32, List<MobDropData>> mobDropTable)
		{
			MapId = mapId;
			TerrainInfo = terrainInfo;
			NpcData = npcData;
			MobSpawnData = mobSpawnData;
			LocalMissionDropData = localMissionDropData;
			LocalDropTable = localDropTable;
			MobDropTable = mobDropTable;
		}

		public int MapId { get; private set; }
		public TerrainInfo TerrainInfo { get; private set; }
		public Dictionary<int, NpcData> NpcData { get; private set; }
		public Dictionary<int, MobSpawnData> MobSpawnData { get; private set; }
		public Dictionary<(int, int, int), MissionDropData> LocalMissionDropData { get; private set; }
		public Dictionary<int, List<LocalDropData>> LocalDropTable { get; private set; }
		public Dictionary<int, List<MobDropData>> MobDropTable { get; private set; }
	}
}
