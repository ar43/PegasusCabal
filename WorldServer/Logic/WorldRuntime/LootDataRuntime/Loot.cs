using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.LootDataRuntime
{
	internal class Loot
	{
		//mapId, dungeonId, speciesId
		public static Dictionary<(int, int, int), List<WorldDropData>>? WorldDropTable { get; private set; }

		public static Dictionary<int, int[]>? OptionPoolLevel { get; private set; }

		public static void LoadConfig(WorldConfig worldConfig)
		{
			if (WorldDropTable != null) throw new Exception("WorldDrop configs already loaded");
			WorldDropTable = new();
			OptionPoolLevel = new();

			var cfg = worldConfig.GetConfig("[WorldDrop]");

			foreach (var worldDrop in cfg.Values)
			{
				int Terrain_World = Convert.ToInt32(worldDrop["Terrain_World"]);
				int DungeonID = Convert.ToInt32(worldDrop["DungeonID"]);
				int Terrain_Mob = Convert.ToInt32(worldDrop["Terrain_Mob"]);
				int ItemKind = Convert.ToInt32(worldDrop["ItemKind"]);
				int ItemOpt = Convert.ToInt32(worldDrop["ItemOpt"]);
				int DropRate = (int)(Convert.ToDouble(worldDrop["DropRate"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				if (DropRate <= 0)
					throw new Exception("unexpected DropRate");
				int MinLv = Convert.ToInt32(worldDrop["MinLv"]);
				int MaxLv = Convert.ToInt32(worldDrop["MaxLv"]);
				int Group = Convert.ToInt32(worldDrop["Group"]);
				int MaxDropCnt = Convert.ToInt32(worldDrop["MaxDropCnt"]);
				int OptPoolIdx = Convert.ToInt32(worldDrop["OptPoolIdx"]);
				int DurationIdx = Convert.ToInt32(worldDrop["DurationIdx"]);
				int DropSvrCh = Convert.ToInt32(worldDrop["DropSvrCh"]);
				int EventDropOnly = Convert.ToInt32(worldDrop["EventDropOnly"]);

				if (!WorldDropTable.ContainsKey((Terrain_World, DungeonID, Terrain_Mob)))
					WorldDropTable[(Terrain_World, DungeonID, Terrain_Mob)] = new();
				WorldDropTable[(Terrain_World, DungeonID, Terrain_Mob)].Add(new WorldDropData(Terrain_World, DungeonID, Terrain_Mob, ItemKind, ItemOpt, DropRate, MinLv, MaxLv, Group, MaxDropCnt, OptPoolIdx, DurationIdx, DropSvrCh, EventDropOnly));
			}

			cfg = worldConfig.GetConfig("[OptionPoolLevel]");
			foreach(var optionPoolLevel in cfg)
			{
				int Lv0 = (int)(Convert.ToDouble(optionPoolLevel.Value["Lv0"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv1 = (int)(Convert.ToDouble(optionPoolLevel.Value["Lv1"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv2 = (int)(Convert.ToDouble(optionPoolLevel.Value["Lv2"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv3 = (int)(Convert.ToDouble(optionPoolLevel.Value["Lv3"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv4 = (int)(Convert.ToDouble(optionPoolLevel.Value["Lv4"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv5 = (int)(Convert.ToDouble(optionPoolLevel.Value["Lv5"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);

				OptionPoolLevel.Add(Convert.ToInt32(optionPoolLevel.Key), new int[] { Lv0, Lv1, Lv2, Lv3, Lv4, Lv5 });
			}
		}
	}

}
