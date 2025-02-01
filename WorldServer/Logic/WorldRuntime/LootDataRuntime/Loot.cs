using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Logic.WorldRuntime.LootDataRuntime
{
	internal struct RareCodeRate
	{
		public int ForceCode;
		public int Rate;
		public int RareNum;
	}
	internal struct ForceRate
	{
		public int ForceCode;
		public int Rate;
	}
	internal class Loot
	{
		//mapId, dungeonId, speciesId
		public static Dictionary<(int, int, int), List<WorldDropData>>? WorldDropTable { get; private set; }

		public static Dictionary<int, int[]>? OptionPoolLevel { get; private set; }
		public static Dictionary<int, int[]>? OptionPoolSlot { get; private set; }
		public static Dictionary<int, int[]>? OptionPoolRareNum { get; private set; }
		public static Dictionary<int, OptionPoolFlag>? OptionPoolFlags { get; private set; }
		//OptPoolId, ForceCode, Nx, Nz
		public static Dictionary<(int,int,int,int), RareCodeRate>? RareCodeRates { get; private set; }
		public static Dictionary<(int, int, int), ForceRate>? ForceRates { get; private set; }

		public static readonly int MAX_SLOTNUM = 3;
		public static readonly int MAX_RARE = 3;
		public static readonly int MAX_RARE_FORCE = 14;
		public static readonly int MAX_LEVEL = 5;
		public static readonly int MAX_FORCE = 6;
		public static readonly int MAX_ITYPE = 7;
		public static readonly int RARENUM_OFFSET = 5;

		public static void LoadConfig(WorldConfig worldConfig)
		{
			if (WorldDropTable != null) throw new Exception("WorldDrop configs already loaded");
			WorldDropTable = new();
			OptionPoolLevel = new();
			OptionPoolFlags = new();
			OptionPoolRareNum = new();
			OptionPoolSlot = new();
			RareCodeRates = new();
			ForceRates = new();

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

			cfg = worldConfig.GetConfig("[OptionPoolSlot]");
			foreach (var optionPoolLevel in cfg)
			{
				int Lv0 = (int)(Convert.ToDouble(optionPoolLevel.Value["Slot0"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv1 = (int)(Convert.ToDouble(optionPoolLevel.Value["Slot1"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv2 = (int)(Convert.ToDouble(optionPoolLevel.Value["Slot2"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv3 = (int)(Convert.ToDouble(optionPoolLevel.Value["Slot3"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv4 = (int)(Convert.ToDouble(optionPoolLevel.Value["SlotOptionNum"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);

				OptionPoolSlot.Add(Convert.ToInt32(optionPoolLevel.Key), new int[] { Lv0, Lv1, Lv2, Lv3, Lv4});
			}

			cfg = worldConfig.GetConfig("[OptionPoolRareNum]");
			foreach (var optionPoolLevel in cfg)
			{
				int Lv0 = (int)(Convert.ToDouble(optionPoolLevel.Value["RareNum0"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv5 = (int)(Convert.ToDouble(optionPoolLevel.Value["RareNum5"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv6 = (int)(Convert.ToDouble(optionPoolLevel.Value["RareNum6"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
				int Lv7 = (int)(Convert.ToDouble(optionPoolLevel.Value["RareNum7"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);

				OptionPoolRareNum.Add(Convert.ToInt32(optionPoolLevel.Key), new int[] { Lv0, Lv5, Lv6, Lv7 });
			}

			cfg = worldConfig.GetConfig("[OptionPoolFlag]");
			foreach (var optionPoolFlag in cfg)
			{
				bool LR = Convert.ToBoolean(Convert.ToInt32(optionPoolFlag.Value["LR"]));
				bool LS = Convert.ToBoolean(Convert.ToInt32(optionPoolFlag.Value["LS"]));
				bool RS = Convert.ToBoolean(Convert.ToInt32(optionPoolFlag.Value["RS"]));
				bool LRS = Convert.ToBoolean(Convert.ToInt32(optionPoolFlag.Value["LRS"]));
				bool UNIQUE = Convert.ToBoolean(Convert.ToInt32(optionPoolFlag.Value["UNIQUE"]));

				OptionPoolFlags.Add(Convert.ToInt32(optionPoolFlag.Key), new(LR, LS, RS, LRS,UNIQUE));
			}

			cfg = worldConfig.GetConfig("[OptionPoolForce]");
			foreach (var optionPoolForce in cfg)
			{
				int OptPoolId = Convert.ToInt32(optionPoolForce.Value["OptPoolIdx"]);
				int ForceCode = Convert.ToInt32(optionPoolForce.Value["ForceCode"]);
				int ForceCodePos = 0;

				if (OptPoolId == 24)
					continue; //TODO!!!!!!!!!!!!!!!!!!!! FIGURE IT OUT

				switch (ForceCode)
				{
					case 0: ForceCodePos = 0; break;
					case 6: ForceCodePos = 1; break;
					case 7: ForceCodePos = 2; break;
					case 8: ForceCodePos = 3; break;
					case 9: ForceCodePos = 4; break;
					case 10: ForceCodePos = 5; break;
					case 15: ForceCodePos = 6; break;
					default: throw new Exception("unexpected");
				}

				string[] cpItemText = ["", "WepRation", "WepRation", "Wep2Ratio", "SuitRatio", "GloveRatio", "BootRatio", "HMetRatio"];

				for(int nx = 1; nx <= MAX_ITYPE; nx++)
				{
					ForceRate forceRate = new ForceRate();

					forceRate.ForceCode = ForceCode;
					forceRate.Rate = (int)(Convert.ToDouble(optionPoolForce.Value[cpItemText[nx]].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);

					ForceRates.Add((OptPoolId, ForceCodePos, nx), forceRate);
				}

			}

			cfg = worldConfig.GetConfig("[OptionPoolRare]");
			foreach (var optionPoolRare in cfg)
			{
				int OptPoolId = Convert.ToInt32(optionPoolRare.Value["OptPoolIdx"]);
				int ForceCode = Convert.ToInt32(optionPoolRare.Value["ForceCode"]);

				string[,] cpItemText = new string[,]{
					{ "", "", ""},
					{ "Wep1Num5Ratio", "Wep1Num6Ratio", "Wep1Num7Ratio" }, 
					{ "Wep1Num5Ratio", "Wep1Num6Ratio", "Wep1Num7Ratio" }, 
					{ "Wep2Num5Ratio", "Wep2Num6Ratio", "Wep2Num7Ratio" }, 
					{ "SuitNum5Ratio", "SuitNum6Ratio",	"SuitNum7Ratio" }, 
					{ "GloveNum5Ratio", "GloveNum6Ratio", "GloveNum7Ratio" },
					{ "BootNum5Ratio", "BootNum6Ratio", "BootNum7Ratio" },
					{ "HelmNum5Ratio", "HelmNum6Ratio",	"HelmNum7Ratio" }
				};

				//TODO: add the rest of weaponms
				// they match the ids of ItemType

				for (int nx = 1; nx <= MAX_ITYPE; nx++)
				{
					for (int nz = 0; nz < 3; nz++)
					{
						RareCodeRate rcr = new();
						rcr.ForceCode = ForceCode;
						rcr.Rate = (int)(Convert.ToDouble(optionPoolRare.Value[cpItemText[nx,nz]].Insert(0, "0"), new CultureInfo("en-US")) / 100.0 * int.MaxValue);
						rcr.RareNum = nz + RARENUM_OFFSET - 1;
						RareCodeRates.Add((OptPoolId, ForceCode-1, nx, nz), rcr);
					}
				}
			}
		}
	}

}
