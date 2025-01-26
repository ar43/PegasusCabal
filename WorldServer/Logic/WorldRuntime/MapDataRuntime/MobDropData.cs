using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MobDropData
	{
		public MobDropData(Int32 terrainIdx, Int32 speciesIdx, Int32 itemKind, Int32 itemOpt, Int32 dropRate, Int32 minLv, Int32 maxDropCnt, Int32 optPoolIdx, Int32 durationIdx)
		{
			TerrainIdx = terrainIdx;
			SpeciesIdx = speciesIdx;
			ItemKind = itemKind;
			ItemOpt = itemOpt;
			DropRate = dropRate;
			MinLv = minLv;
			MaxDropCnt = maxDropCnt;
			OptPoolIdx = optPoolIdx;
			DurationIdx = durationIdx;
		}

		public int TerrainIdx { get; private set; }
		public int SpeciesIdx { get; private set; }
		public int ItemKind { get; private set; }
		public int ItemOpt { get; private set; }
		public int DropRate { get; private set; }
		public int MinLv { get; private set; }
		public int MaxDropCnt { get; private set; }
		public int OptPoolIdx { get; private set; }
		public int DurationIdx { get; private set; }
	}
}
