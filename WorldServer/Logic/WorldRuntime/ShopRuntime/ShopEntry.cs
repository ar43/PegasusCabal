using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.CharData;

namespace WorldServer.Logic.WorldRuntime.ShopRuntime
{
	internal class ShopEntry
	{
		public ShopEntry(Int32 itemKind, Int32 itemOpt, Int32 durationIdx, Int32 minLevel, Int32 maxLevel, Int32 reputation, Int32 onlyPremium, Int32 onlyWin, Int32 alzPrice, Int32 wExpPrice, Int32 dPPrice, Int32 cashPrice, Int32 renew, Int32 characterBuyLimit, Int32 sellLimit, Int32 marker, Int32 maxReputation)
		{
			ItemKind = itemKind;
			ItemOpt = itemOpt;
			DurationIdx = durationIdx;
			MinLevel = minLevel;
			MaxLevel = maxLevel;
			Reputation = reputation;
			OnlyPremium = onlyPremium;
			OnlyWin = onlyWin;
			AlzPrice = alzPrice;
			WExpPrice = wExpPrice;
			DPPrice = dPPrice;
			CashPrice = cashPrice;
			Renew = renew;
			CharacterBuyLimit = characterBuyLimit;
			SellLimit = sellLimit;
			Marker = marker;
			MaxReputation = maxReputation;
		}

		public int ItemKind { get; private set; }
		public int ItemOpt { get; private set; }
		public int DurationIdx { get; private set; }
		public int MinLevel { get; private set; }
		public int MaxLevel { get; private set; }
		public int Reputation { get; private set; }
		public int OnlyPremium { get; private set; }
		public int OnlyWin { get; private set; }
		public int AlzPrice { get; private set; }
		public int WExpPrice { get; private set; }
		public int DPPrice { get; private set; }
		public int CashPrice { get; private set; }
		public int Renew { get; private set; }
		public int CharacterBuyLimit { get; private set; }
		public int SellLimit { get; private set; }
		public int Marker { get; private set; }
		public int MaxReputation { get; private set; }
	}
}
