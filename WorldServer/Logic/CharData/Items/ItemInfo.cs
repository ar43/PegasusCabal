using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.Items
{
	internal class ItemInfo
	{
		public ItemInfo(UInt32 id, String type, Int32 typeId, Int32 priceSell, Int32 width, Int32 height, Int32 opt2_STRLmt1, Int32 dEXLmt1_Opt2Val, Int32 iNTLmt1_Opt3, Int32 opt3Val_STRLmt2, Int32 dEXLmt2_Opt4, Int32 iNTLmt2_Opt4Val, Int32 attckRate_Opt1, Int32 defenRate_Opt1Val_PhyAttMax, Int32 defense_LEVLmt_MagAttVal, Int32 valueLv, Int32 maxCore, Int32 dSTR1, Int32 dDEX1, Int32 dINT1, Int32 dSTR2, Int32 dDEX2, Int32 dINT2, Int32 limitLv, Int32 limitClass, Int32 limitReputation, Int32 grade, Int32 enchantCodeLnk, Int32 property, Int32 periodType, Int32 periodUse, Int32 fixType, Int32 price2, Int32 uniqueGrade, Int32 maxReputation)
		{
			Id = id;
			Type = type;
			TypeId = typeId;
			PriceSell = priceSell;
			Width = width;
			Height = height;
			Opt2_STRLmt1 = opt2_STRLmt1;
			DEXLmt1_Opt2Val = dEXLmt1_Opt2Val;
			INTLmt1_Opt3 = iNTLmt1_Opt3;
			Opt3Val_STRLmt2 = opt3Val_STRLmt2;
			DEXLmt2_Opt4 = dEXLmt2_Opt4;
			INTLmt2_Opt4Val = iNTLmt2_Opt4Val;
			AttckRate_Opt1 = attckRate_Opt1;
			DefenRate_Opt1Val_PhyAttMax = defenRate_Opt1Val_PhyAttMax;
			Defense_LEVLmt_MagAttVal = defense_LEVLmt_MagAttVal;
			ValueLv = valueLv;
			MaxCore = maxCore;
			this.dSTR1 = dSTR1;
			this.dDEX1 = dDEX1;
			this.dINT1 = dINT1;
			this.dSTR2 = dSTR2;
			this.dDEX2 = dDEX2;
			this.dINT2 = dINT2;
			LimitLv = limitLv;
			LimitClass = limitClass;
			LimitReputation = limitReputation;
			Grade = grade;
			EnchantCodeLnk = enchantCodeLnk;
			Property = property;
			PeriodType = periodType;
			PeriodUse = periodUse;
			FixType = fixType;
			Price2 = price2;
			UniqueGrade = uniqueGrade;
			MaxReputation = maxReputation;
		}

		public uint Id { get; private set; }
		public string Type { get; private set; }
		public int TypeId { get; private set; }
		public int PriceSell { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int Opt2_STRLmt1 { get; private set; }
		public int DEXLmt1_Opt2Val { get; private set; }
		public int INTLmt1_Opt3 { get; private set; }
		public int Opt3Val_STRLmt2 { get; private set; }
		public int DEXLmt2_Opt4 { get; private set; }
		public int INTLmt2_Opt4Val { get; private set; }
		public int AttckRate_Opt1 { get; private set; }
		public int DefenRate_Opt1Val_PhyAttMax { get; private set; }
		public int Defense_LEVLmt_MagAttVal { get; private set; }
		public int ValueLv { get; private set; }
		public int MaxCore { get; private set; }
		public int dSTR1 { get; private set; }
		public int dDEX1 { get; private set; }
		public int dINT1 { get; private set; }
		public int dSTR2 { get; private set; }
		public int dDEX2 { get; private set; }
		public int dINT2 { get; private set; }
		public int LimitLv { get; private set; }
		public int LimitClass { get; private set; }
		public int LimitReputation { get; private set; }
		public int Grade { get; private set; }
		public int EnchantCodeLnk { get; private set; }
		public int Property { get; private set; }
		public int PeriodType { get; private set; }
		public int PeriodUse { get; private set; }
		public int FixType { get; private set; }
		public int Price2 { get; private set; }
		public int UniqueGrade { get; private set; }
		public int MaxReputation { get; private set; }
	}
}
