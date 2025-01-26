using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class LocalDropData
	{
		public LocalDropData(Int32 terrainIdx, Int32 itemKind, Int32 itemOpt, Int32 dropRate, Int32 minLv, Int32 maxLv, Int32 group, Int32 maxDropCnt, Int32 optPoolIdx, Int32 durationIdx)
		{
			TerrainIdx = terrainIdx;
			ItemKind = itemKind;
			ItemOpt = itemOpt;
			DropRate = dropRate;
			MinLv = minLv;
			MaxLv = maxLv;
			Group = group;
			MaxDropCnt = maxDropCnt;
			OptPoolIdx = optPoolIdx;
			DurationIdx = durationIdx;
		}

		public int TerrainIdx { get; private set; }
		public int ItemKind { get; private set; }
		public int ItemOpt { get; private set; }
		public int DropRate { get; private set; }
		public int MinLv { get; private set; }
		public int MaxLv { get; private set; }
		public int Group { get; private set; }
		public int MaxDropCnt { get; private set; }
		public int OptPoolIdx { get; private set; }
		public int DurationIdx { get; private set; }

	}
}
