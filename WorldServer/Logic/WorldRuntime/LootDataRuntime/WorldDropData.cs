using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.LootDataRuntime
{
	internal class WorldDropData
	{
		public WorldDropData(Int32 terrain_World, Int32 dungeonID, Int32 terrain_Mob, Int32 itemKind, Int32 itemOpt, Int32 dropRate, Int32 minLv, Int32 maxLv, Int32 group, Int32 maxDropCnt, Int32 optPoolIdx, Int32 durationIdx, Int32 dropSvrCh, Int32 eventDropOnly)
		{
			Terrain_World = terrain_World;
			DungeonID = dungeonID;
			Terrain_Mob = terrain_Mob;
			ItemKind = itemKind;
			ItemOpt = itemOpt;
			DropRate = dropRate;
			MinLv = minLv;
			MaxLv = maxLv;
			Group = group;
			MaxDropCnt = maxDropCnt;
			OptPoolIdx = optPoolIdx;
			DurationIdx = durationIdx;
			DropSvrCh = dropSvrCh;
			EventDropOnly = eventDropOnly;
		}

		public int Terrain_World { get; private set; }
		public int DungeonID { get; private set; }
		public int Terrain_Mob { get; private set; }
		public int ItemKind { get; private set; }
		public int ItemOpt { get; private set; }
		public int DropRate { get; private set; }
		public int MinLv { get; private set; }
		public int MaxLv { get; private set; }
		public int Group { get; private set; }
		public int MaxDropCnt { get; private set; }
		public int OptPoolIdx { get; private set; }
		public int DurationIdx { get; private set; }
		public int DropSvrCh { get; private set; }
		public int EventDropOnly { get; private set; }
	}
}
