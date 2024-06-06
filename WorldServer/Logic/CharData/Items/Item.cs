using Shared.Protos;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime;

namespace WorldServer.Logic.CharData.Items
{
	internal class Item
	{
		public Item(UInt32 kind, UInt32 option, UInt32 serial, UInt32 duration)
		{
			if (ItemConfig == null)
				throw new Exception("item configs not yet loaded");
			Kind = kind;
			Option = option;
			Serial = serial;
			Duration = duration;
			_itemInfo = ItemConfig[GetId()];
		}

		public UInt32 Kind { get; private set; }

		public UInt32 Option { get; private set; }
		public UInt32 Serial { get; private set; }
		public UInt32 Duration { get; private set; } //change to period

		private static Dictionary<UInt32, ItemInfo>? ItemConfig;
		private ItemInfo _itemInfo;

		public int GetWidth()
		{
			return _itemInfo.Width;
		}

		public int GetHeight()
		{
			return _itemInfo.Height;
		}

		public int GetAttack()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_1HND || _itemInfo.TypeId == ItemType.IDT_2HND);
			return _itemInfo.DefenRate_Opt1Val_PhyAttMax;
		}

		public int GetMagicAttack()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_1HND || _itemInfo.TypeId == ItemType.IDT_2HND);
			return _itemInfo.Defense_LEVLmt_MagAttVal;
		}

		public bool TryUse()
		{
			switch(_itemInfo.TypeId)
			{
				case ItemType.IDT_WSCL:
				{
					if (Option == 0)
						return false;
					Option--;
					break;
				}
				default:
				{
					Serilog.Log.Error($"Item.TryUse: Unimplemented type {_itemInfo.TypeId}");
					return false;
				}
			}
			return true;
		}

		public void SetKind(UInt32 kind)
		{
			Kind = kind;
		}

		public void SetOption(UInt32 option)
		{
			Option = option;
		}

		public uint GetId()
		{
			return Kind & (1 << 12) - 1;
		}

		public ItemData GetProtobuf()
		{
			ItemData data = new ItemData { Kind = Kind, Option = Option, Serial = Serial, Duration = Duration };

			return data;
		}

		public static void LoadConfigs(WorldConfig worldConfig)
		{
			if (ItemConfig != null) throw new Exception("item configs already loaded");
			ItemConfig = new();

			var cfg = worldConfig.GetConfig("[Item]");

			foreach (var item in cfg)
			{
				var itemId = Convert.ToUInt32(item.Key);
				if (itemId == 0)
					continue;

				var typeStr = item.Value["Type"];
				if (!Enum.TryParse(typeStr.Substring(1), out ItemType typeNum))
					throw new Exception("Invalid type");
				if (!Int32.TryParse(item.Value["PriceSell"], out var priceSell))
					priceSell = 0;
				if (!Int32.TryParse(item.Value["Width"], out var width))
					width = 0;
				if (!Int32.TryParse(item.Value["Height"], out var height))
					height = 0;
				if (!Int32.TryParse(item.Value["Opt2/STRLmt1"], out var opt2_strlmt1))
					opt2_strlmt1 = 0;
				if (!Int32.TryParse(item.Value["DEXLmt1/Opt2Val"], out var dexlmt1_opt2val))
					dexlmt1_opt2val = 0;
				if (!Int32.TryParse(item.Value["INTLmt1/Opt3"], out var intlmt1_opt3))
					intlmt1_opt3 = 0;
				if (!Int32.TryParse(item.Value["Opt3Val/STRLmt2"], out var opt3val_strlmt2))
					opt3val_strlmt2 = 0;
				if (!Int32.TryParse(item.Value["DEXLmt2/Opt4"], out var dexlmt2_opt4))
					dexlmt2_opt4 = 0;
				if (!Int32.TryParse(item.Value["INTLmt2/Opt4Val"], out var intlmt2_opt4val))
					intlmt2_opt4val = 0;
				if (!Int32.TryParse(item.Value["AttckRate/Opt1"], out var attckrate_opt1))
					attckrate_opt1 = 0;
				if (!Int32.TryParse(item.Value["DefenRate/Opt1Val/PhyAttMax"], out var defenrate_opt1val_phyattmax))
					defenrate_opt1val_phyattmax = 0;
				if (!Int32.TryParse(item.Value["Defense/LEVLmt/MagAttVal"], out var defense_levlmt_magattval))
					defense_levlmt_magattval = 0;
				if (!Int32.TryParse(item.Value["ValueLv"], out var valueLv))
					valueLv = 0;
				if (!Int32.TryParse(item.Value["MaxCore"], out var maxCore))
					maxCore = 0;
				if (!Int32.TryParse(item.Value["dSTR1"], out var dstr1))
					dstr1 = 0;
				if (!Int32.TryParse(item.Value["dDEX1"], out var ddex1))
					ddex1 = 0;
				if (!Int32.TryParse(item.Value["dINT1"], out var dint1))
					dint1 = 0;
				if (!Int32.TryParse(item.Value["dSTR2"], out var dstr2))
					dstr2 = 0;
				if (!Int32.TryParse(item.Value["dDEX2"], out var ddex2))
					ddex2 = 0;
				if (!Int32.TryParse(item.Value["dINT2"], out var dint2))
					dint2 = 0;
				if (!Int32.TryParse(item.Value["LimitLv"], out var limitLv))
					limitLv = 0;
				if (!Int32.TryParse(item.Value["LimitClass"], out var limitClass))
					limitClass = 0;
				if (!Int32.TryParse(item.Value["LimitReputation"], out var limitReputation))
					limitReputation = 0;
				if (!Int32.TryParse(item.Value["Grade"], out var grade))
					grade = 0;
				if (!Int32.TryParse(item.Value["EnchantCodeLnk"], out var enchantCodeLnk))
					enchantCodeLnk = 0;
				if (!Int32.TryParse(item.Value["Property"], out var property))
					property = 0;
				if (!Int32.TryParse(item.Value["PeriodType"], out var periodType))
					periodType = 0;
				if (!Int32.TryParse(item.Value["PeriodUse"], out var periodUse))
					periodUse = 0;
				if (!Int32.TryParse(item.Value["FixType"], out var fixType))
					fixType = 0;
				if (!Int32.TryParse(item.Value["Price2"], out var price2))
					price2 = 0;
				if (!Int32.TryParse(item.Value["UniqueGrade"], out var uniqueGrade))
					uniqueGrade = 0;
				if (!Int32.TryParse(item.Value["MaxReputation"], out var maxReputation))
					maxReputation = 0;

				ItemInfo itemInfo = new(itemId, typeStr, typeNum, priceSell, width, height, opt2_strlmt1, dexlmt1_opt2val, intlmt1_opt3, opt3val_strlmt2, dexlmt2_opt4, intlmt2_opt4val,
					attckrate_opt1, defenrate_opt1val_phyattmax, defense_levlmt_magattval, valueLv, maxCore, dstr1, ddex1, dint1, dstr2, ddex2, dint2, limitLv, limitClass,
					limitReputation, grade, enchantCodeLnk, property, periodType, periodUse, fixType, price2, uniqueGrade, maxReputation);
				ItemConfig.Add(itemId, itemInfo);

			}


		}

		internal Int32 GetAttackRate()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_1HND || _itemInfo.TypeId == ItemType.IDT_2HND);
			return _itemInfo.AttckRate_Opt1;
		}

		internal Int32 GetCR()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_RING);
			return _itemInfo.DefenRate_Opt1Val_PhyAttMax;
		}
	}
}
