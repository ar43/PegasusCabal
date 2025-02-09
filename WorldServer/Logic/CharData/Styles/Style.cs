using LibPegasus.Utils;
using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.CharData.Styles.Coefs;
using WorldServer.Logic.WorldRuntime;

namespace WorldServer.Logic.CharData.Styles
{
	internal class Style
	{
		public byte BattleStyleNum { get; private set; }
		public byte MasteryLevel { get; private set; }
		public byte Face { get; private set; }
		public byte HairColor { get; private set; }
		public byte HairStyle { get; private set; }
		public byte Aura { get; private set; }
		public byte Gender { get; private set; }
		public byte ShowHelmet { get; private set; }

		public BattleStyle BattleStyle { get; private set; }
		public StyleEx StyleEx { get; private set; }
		private static Dictionary<int, BattleStyle>? _battleStyleData;
		public DBSyncPriority SyncPending { get; private set; }

		public Style(Byte battleStyle, Byte masteryLevel, Byte face, Byte hairColor, Byte hairStyle, Byte aura, Byte gender, Byte showHelmet)
		{
			if (_battleStyleData == null)
				throw new Exception("Data not yet loaded");

			BattleStyleNum = battleStyle; //3
			BattleStyle = _battleStyleData[BattleStyleNum];
			MasteryLevel = masteryLevel; //5
			Face = face; //5
			HairColor = hairColor; //4
			HairStyle = hairStyle; ///5
			Aura = aura; //4
			Gender = gender; //1
			ShowHelmet = showHelmet; //1
			StyleEx = new();

			SyncPending = DBSyncPriority.NONE;
		}

		public Style(UInt32 serial)
		{
			if (_battleStyleData == null)
				throw new Exception("Data not yet loaded");

			BattleStyleNum = (Byte)(serial & 0b111);
			MasteryLevel = (Byte)(serial >> 3 & 0b11111);
			Face = (Byte)(serial >> 8 & 0b11111);
			HairColor = (Byte)(serial >> 13 & 0b1111);
			HairStyle = (Byte)(serial >> 17 & 0b11111);
			Aura = (Byte)(serial >> 22 & 0b1111);
			Gender = (Byte)(serial >> 26 & 0b1);
			ShowHelmet = (Byte)(serial >> 27 & 0b1);

			StyleEx = new();

			BattleStyle = _battleStyleData[BattleStyleNum];

			if (MasteryLevel == 0)
				throw new Exception("Not supposed to be 0");

			SyncPending = DBSyncPriority.NONE;
		}

		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public DbSyncStyle GetDB()
		{
			DbSyncStyle style = new DbSyncStyle(Serialize());
			return style;
		}

		public void SetAura(byte auraCode)
		{
			Aura = auraCode;
		}

		public UInt32 Serialize()
		{
			UInt32 result = 0;
			result |= BattleStyleNum;
			result |= (UInt32)MasteryLevel << 3;
			result |= (UInt32)Face << 8;
			result |= (UInt32)HairColor << 13;
			result |= (UInt32)HairStyle << 17;
			result |= (UInt32)Aura << 22;
			result |= (UInt32)Gender << 26;
			result |= (UInt32)ShowHelmet << 27;

			return result;
		}

		public bool Verify()
		{
			//TODO
			return true;
		}

		public bool VerifyOnCreate()
		{
			if (MasteryLevel != 1)
				return false;

			return true;
		}

		public void ToggleHelmet(byte newVal)
		{
			ShowHelmet = newVal;
		}

		public void SetMasteryLevel(byte value)
		{
			MasteryLevel = value;
		}

		public int CalculateValueFromCoef(StyleCoef coef)
		{
			return coef.A * MasteryLevel + coef.B;
		}

		public static void LoadConfigs(WorldConfig worldConfig)
		{
			if (_battleStyleData != null)
				throw new Exception("Battle styles already initialized");
			_battleStyleData = new();

			var cfg = worldConfig.GetConfig("[BattleStyleData]");

			foreach (var it in cfg)
			{
				int BattleStyleId = Convert.ToInt32(it.Key);
				if (BattleStyleId <= 0)
					continue;

				StyleCoef AttackCoef = new(Utility.StringToIntArray(it.Value["AttackCoef"]));
				StyleCoef MagAttCoef = new(Utility.StringToIntArray(it.Value["MagAttCoef"]));
				StyleCoef DefensCoef = new(Utility.StringToIntArray(it.Value["DefensCoef"]));
				StyleCoef AttckRCoef = new(Utility.StringToIntArray(it.Value["AttckRCoef"]));
				StyleCoef DefenRCoef = new(Utility.StringToIntArray(it.Value["DefenRCoef"]));
				StatCoef StatMaxAtt = new(Utility.StringToIntArray(it.Value["StatMaxAtt"]));
				StatCoef StatMagAtt = new(Utility.StringToIntArray(it.Value["StatMagAtt"]));
				StatCoef StatDefens = new(Utility.StringToIntArray(it.Value["StatDefens"]));
				StatCoef StatAttckR = new(Utility.StringToIntArray(it.Value["StatAttckR"]));
				StatCoef StatDefenR = new(Utility.StringToIntArray(it.Value["StatDefenR"]));
				int InitHP = Convert.ToInt32(it.Value["InitHP"]);
				int InitMP = Convert.ToInt32(it.Value["InitMP"]);
				int DeltaHP = Convert.ToInt32(it.Value["DeltaHP"]);
				int DeltaMP = Convert.ToInt32(it.Value["DeltaMP"]);
				int MinDmgRate = Convert.ToInt32(it.Value["MinDmgRate"]);

				BattleStyle battleStyle = new(BattleStyleId, AttackCoef, MagAttCoef, DefensCoef, AttckRCoef, DefenRCoef, StatMaxAtt, StatMagAtt,
					StatDefens, StatAttckR, StatDefenR, InitHP, InitMP, DeltaHP, DeltaMP, MinDmgRate);

				_battleStyleData.Add(BattleStyleId, battleStyle);
			}


		}
	}
}
