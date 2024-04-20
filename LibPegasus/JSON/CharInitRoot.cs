using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.JSON
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public class CharInitRoot
	{
		public CharInitData[] CharInitData { get; set; }
	}

	public class CharInitData
	{
		public int ClassType { get; set; }
		public int LEV { get; set; }
		public int Exp { get; set; }
		public int STR { get; set; }
		public int DEX { get; set; }
		public int INT { get; set; }
		public int PNT { get; set; }
		public int Rank { get; set; }
		public int Alz { get; set; }
		public int WorldIdx { get; set; }
		public PositionDataEntry Position { get; set; }
		public int HP { get; set; }
		public int MP { get; set; }
		public int SP { get; set; }
		public int SwdPNT { get; set; }
		public int MagPNT { get; set; }
		public int RankEXP { get; set; }
		public int Flags { get; set; }
		public int WarpBField { get; set; }
		public int MapsBField { get; set; }
		public Dictionary<UInt32, InventoryDataItem> InventoryData { get; set; }
		public Dictionary<UInt32, EquipmentDataItem> EquipmentData { get; set; }
		public Dictionary<UInt16, SkillDataEntry> SkillData { get; set; }
		public Dictionary<UInt16, QuickSlotDataEntry> QuickSlotData { get; set; }
		public int QuestData { get; set; }
		public string QuestFlagsData { get; set; }
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
