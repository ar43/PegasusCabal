using Shared.Protos;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime;
using WorldServer.Logic.WorldRuntime.LootDataRuntime;

namespace WorldServer.Logic.CharData.Items
{
	internal class Item
	{
		public Item(UInt32 kind, UInt32 option, UInt32 serial, UInt32 duration)
		{
			if (ItemConfig == null || _itemRewardData == null)
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
		private static ItemRewardData? _itemRewardData = null;
		private ItemInfo _itemInfo;

		public readonly uint MASK_QITEM_CNT = 0x7f;
		public readonly uint MASK_QITEM_IDX = 0xFFF80;
		public readonly uint MASK_ITEMKINDINDEX = 0x00000FFF;
		public readonly uint MASK_ITEMKOWNERSHIP = 0x00001000;

		public readonly uint SHF_UPGRADECORE = 13;
		public readonly uint SHF_MAXOPTSCORE = 28;
		public readonly uint SHF_OPTCORENUMB = 4;
		public readonly uint SHF_OWNERSHIP = 12;

		public bool IsItemOpt()
		{
			return (GetItemType() == ItemType.IDT_QSTS || GetItemType() == ItemType.IDT_SPOS) ? true : false;
		}

		private bool SelectSlot(Random rng, int optionPoolId)
		{
			int roll = rng.Next();
			int slotNum = 0, forceCode = 0;

			if (optionPoolId == 24)
				throw new NotImplementedException();

			Loot.OptionPoolSlot.TryGetValue(optionPoolId, out var optionPoolSlot);
			if (optionPoolSlot == null)
				return false;

			//TODO: bonus slot chance

			for (int i = 0; i < Loot.MAX_SLOTNUM; i++)
			{
				int slotRate = optionPoolSlot[i];
				int slotRate0 = optionPoolSlot[0];
				int slotRate1 = optionPoolSlot[1];

				if(roll < slotRate)
				{
					if(i > 0)
					{
						Option |= (UInt32)(i << (int)SHF_MAXOPTSCORE);
						roll = rng.Next();

						for(int nx = 0; nx <= Loot.MAX_FORCE; nx++)
						{
							if (Loot.ForceRates.TryGetValue((optionPoolId, nx, (int)GetItemType()), out var forceRate))
							{
								if(roll < forceRate.Rate)
								{
									if (forceRate.ForceCode > 0)
									{
										slotNum = i;
										forceCode = forceRate.ForceCode;
										if ((Option & 0x80) > 0)
										{
											Option |= (UInt32)((forceCode | (1 << (int)SHF_OPTCORENUMB)) << 8);
										}
										else
										{
											Option |= (UInt32)(forceCode | (1 << (int)SHF_OPTCORENUMB));
										}
									}
									break;
								}
							}
							else
							{
								throw new NotImplementedException();
							}
						}
					}
					break;
				}
			}

			return (slotNum > 0) ? true : false;
		}

		private bool SelectRare(Random rng, int optionPoolId)
		{
			int roll = rng.Next();
			int rareNum = 0;
			int forceCode = 0;
			Loot.OptionPoolRareNum.TryGetValue(optionPoolId, out var optionPoolRareNum);
			if (optionPoolRareNum == null)
				return false;

			for (int i = 0; i < optionPoolRareNum.Length; i++)
			{
				if (roll < optionPoolRareNum[i])
				{
					if(i > 0)
					{
						roll = rng.Next();

						for(int nx = 0; nx <= Loot.MAX_RARE_FORCE; nx++)
						{
							if(Loot.RareCodeRates.TryGetValue((optionPoolId, nx, (int)GetItemType(), i), out var rareCodeRate))
							{
								if(roll < rareCodeRate.Rate)
								{
									if(rareCodeRate.RareNum > 0)
									{
										rareNum = rareCodeRate.RareNum;
										forceCode = rareCodeRate.ForceCode;
										Option |= 0x80;
										Option |= (UInt32)(rareNum << (int)SHF_OPTCORENUMB);
										Option |= (UInt32)forceCode;
									}
									break;
								}
							}
							else
							{
								throw new NotImplementedException();
							}
						}
					}
					break;
				}
			}

			return (rareNum > 0) ? true : false;
		}

		public void GenerateOption(Random rng, int optionPoolId)
		{
			if (IsItemOpt() || _itemInfo.MaxCore <= 0 || optionPoolId == 0)
			{
				Serilog.Log.Debug($"Refused option: {IsItemOpt()}, {_itemInfo.MaxCore}, {optionPoolId}");
				return;
			}
				

			//TODO: serial
			int roll = rng.Next();
			uint i;

			Loot.OptionPoolLevel.TryGetValue(optionPoolId, out var optionPoolLevel);
			if (optionPoolLevel == null)
				return;

			Debug.Assert(optionPoolLevel.Length == 6);

			for (i = 0; i < optionPoolLevel.Length; i++)
			{
				if(roll < optionPoolLevel[i])
				{
					if (i > 0)
						SetKind((Kind & MASK_ITEMKINDINDEX) | (i << (int)SHF_UPGRADECORE));
					break;
				}
			}

			Loot.OptionPoolFlags.TryGetValue(optionPoolId, out var optionPoolFlag);
			if (optionPoolFlag == null)
				return;

			if (i > 0)
			{
				if(optionPoolFlag.LR)
				{
					if(SelectRare(rng, optionPoolId))
					{
						if (optionPoolFlag.LRS)
							SelectSlot(rng, optionPoolId);
					}
					else
					{
						if(optionPoolFlag.LS)
							SelectSlot(rng, optionPoolId);
					}
				}
				else
				{
					if(optionPoolFlag.LS)
						SelectSlot(rng, optionPoolId);
				}
			}
			else
			{
				if(SelectRare(rng, optionPoolId))
				{
					if(optionPoolFlag.RS)
						SelectSlot(rng, optionPoolId);
				}
				else
				{
					SelectSlot(rng, optionPoolId);
				}
			}

		}

		public bool IsQuestItem()
		{
			return GetItemType() == ItemType.IDT_QSTS;
		}

		public int GetWidth()
		{
			return _itemInfo.Width;
		}

		public void ConvertOptionToQuestOption(int quantity)
		{
			Debug.Assert(IsQuestItem());
			Option = (uint)((Option << 7) + (quantity & MASK_QITEM_CNT));
		}

		public void SetQuestItemCount(int quantity)
		{
			Debug.Assert(IsQuestItem());
			Option &= MASK_QITEM_IDX;
			Option += (UInt32)(quantity & MASK_QITEM_CNT);
		}

		public uint GetQuestItemOpt()
		{
			Debug.Assert(IsQuestItem());
			return (Option & MASK_QITEM_IDX) >> 7;
		}

		public uint GetQuestItemCount()
		{
			Debug.Assert(IsQuestItem());
			return Option & MASK_QITEM_CNT;
		}

		public int GetHeight()
		{
			return _itemInfo.Height;
		}

		public int GetAttack()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_1HND || _itemInfo.TypeId == ItemType.IDT_2HND || _itemInfo.TypeId == ItemType.IDT_MWPN);
			return _itemInfo.DefenRate_Opt1Val_PhyAttMax;
		}

		public ItemType GetItemType()
		{
			return _itemInfo.TypeId;
		}

		public int GetMagicAttack()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_1HND || _itemInfo.TypeId == ItemType.IDT_2HND || _itemInfo.TypeId == ItemType.IDT_MWPN);
			return _itemInfo.Defense_LEVLmt_MagAttVal;
		}

		public bool TryUse()
		{
			switch (_itemInfo.TypeId)
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

		public static Item GenerateReward(uint rewardItemIdx, uint battleStyle, uint order)
		{
			if (_itemRewardData.MainData.TryGetValue(new(rewardItemIdx, battleStyle, order), out var reward))
			{
				Item item = new(reward.ItemKind, reward.ItemOpt, 0, reward.Duration);
				return item;
			}
			else
			{
				if (_itemRewardData.MainData.TryGetValue(new(rewardItemIdx, 0, order), out var reward2))
				{
					Item item = new(reward2.ItemKind, reward2.ItemOpt, 0, reward2.Duration);
					return item;
				}
				else
				{
					throw new Exception("Item could not be generated");
				}
			}

		}

		public static void LoadItemRewards(WorldConfig worldConfig)
		{
			if (_itemRewardData != null) throw new Exception("item reward config already loaded");
			_itemRewardData = new();

			var cfg = worldConfig.GetConfig("[RewardItem]");
			foreach (var it in cfg.Values)
			{
				uint RewardItemIdx = Convert.ToUInt32(it["RewardItemIdx"]);
				uint Class = Convert.ToUInt32(it["Class"]);
				string Type = new(it["Type"]);
				uint Grade = Convert.ToUInt32(it["Grade"]);
				uint Order = Convert.ToUInt32(it["Order"]);
				uint ItemKind = Convert.ToUInt32(it["ItemKind"]);
				int ItemOpt = Convert.ToInt32(it["ItemOpt"]);
				uint Duration = Convert.ToUInt32(it["Duration"]);
				_itemRewardData.Add(new(RewardItemIdx, Class, Order), new(RewardItemIdx, Class, Type, Grade, Order, ItemKind, (uint)ItemOpt, Duration));
			}
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
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_1HND || _itemInfo.TypeId == ItemType.IDT_2HND || _itemInfo.TypeId == ItemType.IDT_MWPN);
			return _itemInfo.AttckRate_Opt1;
		}

		internal Int32 GetOpt1()
		{
			return _itemInfo.AttckRate_Opt1;
		}

		internal Int32 GetOpt1Val()
		{
			return _itemInfo.DefenRate_Opt1Val_PhyAttMax;
		}

		internal Int32 GetDefense()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_SUIT || _itemInfo.TypeId == ItemType.IDT_HMET || _itemInfo.TypeId == ItemType.IDT_GLOV || _itemInfo.TypeId == ItemType.IDT_BOOT);
			return _itemInfo.Defense_LEVLmt_MagAttVal;
		}

		internal Int32 GetDefenseRate()
		{
			Debug.Assert(_itemInfo.TypeId == ItemType.IDT_SUIT || _itemInfo.TypeId == ItemType.IDT_HMET || _itemInfo.TypeId == ItemType.IDT_GLOV || _itemInfo.TypeId == ItemType.IDT_BOOT);
			return _itemInfo.DefenRate_Opt1Val_PhyAttMax;
		}

		internal Int32 GetGrade()
		{
			return _itemInfo.Grade;
		}
	}
}
