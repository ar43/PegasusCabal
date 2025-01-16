namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal record MissionDropData
	{
		public MissionDropData(Int32 terrainIdx, Int32 speciesIdx, Int32 itemKind, Int32 itemOpt, Int32 dropRate, Int32 maxDropCnt)
		{
			TerrainIdx = terrainIdx;
			SpeciesIdx = speciesIdx;
			ItemKind = itemKind;
			ItemOpt = itemOpt;
			DropRate = dropRate;
			MaxDropCnt = maxDropCnt;
		}

		public int TerrainIdx { get; init; }
		public int SpeciesIdx { get; init; }
		public int ItemKind { get; init; }
		public int ItemOpt { get; init; }
		public int DropRate { get; init; }
		public int MaxDropCnt { get; init; }
	}
}
