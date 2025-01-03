using LibPegasus.Utils;
using System.ComponentModel;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.CharData.Styles;
using WorldServer.Logic.WorldRuntime;

namespace WorldServer.Logic.CharData.Skills
{
    internal class Skill
    {
        public Skill(UInt16 id, byte level)
        {
			if (SkillConfig == null)
				throw new Exception("Skill Data not yet loaded");
            Id = id;
            Level = level;
			_skillInfoMain = SkillConfig.MainData[id];
        }

        public UInt16 Id { get; private set; }
        public byte Level { get; private set; }

        private static SkillInfo? SkillConfig = null;
        private readonly SkillInfoMain _skillInfoMain;

		public int GetSkillExp(int id)
		{
			Debug.Assert(id == 1 || id == 2);
			if(id == 1)
				return _skillInfoMain.SkillExp1;
			else
				return _skillInfoMain.SkillExp2;
		}

		public List<BuffData>? GetSkillBuffData()
		{
			var valueList = _skillInfoMain.SkillValueList;

			if (valueList.Count == 0)
				return null;

			List<BuffData> buffData = new List<BuffData>();

			foreach(var it in valueList)
			{
				buffData.Add(new((ForceCode)it.BForce_ID, it.BaseDiv(Level)));
			}

			return buffData;
		}

		public SkillGroup GetGroup()
		{
			return _skillInfoMain.Group;
		}

		public int GetExclusive()
		{
			return _skillInfoMain.Exclusive;
		}

		public int GetUseCase()
		{
			return _skillInfoMain.UseCase;
		}

		public bool CheckUseCase(SkillUseCase useCase)
		{
			return (GetUseCase() & (uint)useCase) == (uint)useCase;
		}

		public bool IsSwordSkill()
		{
			return _skillInfoMain.Type == Enums.SkillType.ST_SWRDSKIL;
		}

		public bool IsMagicSkill()
		{
			return _skillInfoMain.Type == Enums.SkillType.ST_MAGCSKIL;
		}

		private int DeltaSwordAmp()
		{
            if (IsSwordSkill())
            {
				return (int)(Level / 10) + (int)(Level / 13) + (int)(Level / 16) + (int)(Level / 19);
			}
			else
			{
				return 0;
			}
        }

		private int DeltaMagicAmp()
		{
			if (IsMagicSkill())
			{
				return (int)(Level / 10) + (int)(Level / 13) + (int)(Level / 16) + (int)(Level / 19);
			}
			else
			{
				return 0;
			}
		}

		public int CalculateAttackRate(int attackRate)
		{
			return attackRate + (_skillInfoMain.ARatingCoef.CoefA * Level + _skillInfoMain.ARatingCoef.CoefB); 
		}

		public int CalculateAttack(int attack, int swordAmp, int magicAttack, int magicAmp)
		{
			return (((5 * 2 * _skillInfoMain.AttackCoef.CoefA + 5 * DeltaSwordAmp() + swordAmp) * attack) +
					 ((5 * 2 * _skillInfoMain.AttackCoef.CoefB + 5 * DeltaMagicAmp() + magicAmp) * magicAttack) +
					 (5 * 2 * _skillInfoMain.AttackCoef.CoefC * Level) +
					 (5 * 2 * _skillInfoMain.AttackCoef.CoefD)) / (5 * 20);
		}

		private int GetBaseCriticalRate()
		{
			return _skillInfoMain.CriticalRateCoef.CoefA * Level + _skillInfoMain.CriticalRateCoef.CoefB;
		}

		private int GetBaseCriticalDamage()
		{
			return _skillInfoMain.CriticalMultiCoef.CoefA * Level + _skillInfoMain.CriticalMultiCoef.CoefB;
		}

		private int GetBasCriticaleDamage()
		{
			return _skillInfoMain.CriticalMultiCoef.CoefA * Level + _skillInfoMain.CriticalMultiCoef.CoefB;
		}

		public int CalculateCritRate(int criticalRate, int maxCriticalRate)
		{
			switch(_skillInfoMain.Group)
			{
				case Enums.SkillGroup.SK_GROUP001:
				case Enums.SkillGroup.SK_GROUP002:
				case Enums.SkillGroup.SK_GROUP003:
				case Enums.SkillGroup.SK_GROUP004:
				case Enums.SkillGroup.SK_GROUP014:
				{
					criticalRate += GetBaseCriticalRate();
					criticalRate = Math.Min(criticalRate, maxCriticalRate);
					return criticalRate;
				}
				default:
				{
					return criticalRate;
				}
			}
		}

		public static void LoadConfigs(WorldConfig worldConfig)
		{
			if (SkillConfig != null) throw new Exception("item configs already loaded");
			SkillConfig = new();

			var cfg = worldConfig.GetConfig("[SKill_Main]");

			foreach (var it in cfg.Values)
			{
				int SkillIdx = Convert.ToInt32(it["SkillIdx"]);
				int Multi = Convert.ToInt32(it["Multi"]);
				int Target = Convert.ToInt32(it["Target"]);
				int Max_Target = Convert.ToInt32(it["Max_Target"]);
				int Type = Convert.ToInt32(it["Type"]);
				int Group = Convert.ToInt32(it["Group"]);
				int[]? Dur = Utility.StringToIntArray(it["Dur"]);
				int BFX_Dur = 0;
				if (it["BFX_Dur"] != "<null>")
					BFX_Dur = Convert.ToInt32(it["BFX_Dur"]);
				int Value_Ref = Convert.ToInt32(it["Value_Ref"]);
				int Bfx_Ref = Convert.ToInt32(it["Bfx_Ref"]);
				int Property = Convert.ToInt32(it["Property"]);
				int Element = Convert.ToInt32(it["Element"]);
				int SkillExp1 = 0;
				if (it["SkillExp1"] != "<null>")
					SkillExp1 = Convert.ToInt32(it["SkillExp1"]);
				if (!Int32.TryParse(it["SkillExp2"], out var SkillExp2))
					SkillExp2 = 0;
				int Grip = Convert.ToInt32(it["Grip"]);
				int Exclusive = Convert.ToInt32(it["Exclusive"]);
				int UseCase = Convert.ToInt32(it["UseCase"]);
				if (!Int32.TryParse(it["Reach"], out var Reach))
					Reach = 0;
				if (!Int32.TryParse(it["Range"], out var Range))
					Range = 0;
				if (!Int32.TryParse(it["RangeType"], out var RangeType))
					RangeType = 0;
				if (!Int32.TryParse(it["FiringFrame"], out var FiringFrame))
					FiringFrame = 0;
				if (!Int32.TryParse(it["HitFrame"], out var HitFrame))
					HitFrame = 0;
				int Move_Method = Convert.ToInt32(it["Move_Method"]);
				SkillCoef AttackCoef = new SkillCoef(Utility.StringToIntArray(it["AttackCoef"]));
				int[]? ARatingCoef = Utility.StringToIntArray(it["A.RatingCoef"]);
				int[]? CriticalRateCoef = Utility.StringToIntArray(it["CriticalRateCoef"]);
				int[]? CriticalMultiCoef = Utility.StringToIntArray(it["CriticalMultiCoef"]);
				int[]? MpCoef = Utility.StringToIntArray(it["MpCoef"]);
				int[]? MPWasteBModeCoef = Utility.StringToIntArray(it["MPWasteBModeCoef"]);
				int SPWasteVal = Convert.ToInt32(it["SPWasteVal"]);
				int checklimit_normal = Convert.ToInt32(it["checklimit_normal"]);
				int checklimit_combo = Convert.ToInt32(it["checklimit_combo"]);
				int Upkeepctime = Convert.ToInt32(it["Upkeepctime"]);
				int Rate = Convert.ToInt32(it["Rate"]);
				int AmpCount = Convert.ToInt32(it["AmpCount"]);
				int DamageType = Convert.ToInt32(it["DamageType"]);

				SkillInfoMain data = new(SkillIdx, Multi, Target, Max_Target, Type, Group, Dur, BFX_Dur,
					Value_Ref, Bfx_Ref, Property, Element, SkillExp1, SkillExp2, Grip, Exclusive,
					UseCase, Reach, Range, RangeType, FiringFrame, HitFrame, Move_Method, AttackCoef, ARatingCoef, CriticalRateCoef, CriticalMultiCoef, MpCoef,
					MPWasteBModeCoef, SPWasteVal, checklimit_normal, checklimit_combo, Upkeepctime, Rate, AmpCount, DamageType);
				SkillConfig.Add(SkillIdx, data);
			}

			cfg = worldConfig.GetConfig("[SKill_Value]");
			foreach (var it in cfg.Values)
			{
				int SkillIdx = Convert.ToInt32(it["SkillIdx"]);
				int Group = Convert.ToInt32(it["Group"]);
				int Order = Convert.ToInt32(it["Order"]);
				int BForce_ID = Convert.ToInt32(it["BForce_ID"]);
				int[]? Value = Utility.StringToIntArray(it["Value"]);
				if (!Int32.TryParse(it["Power"], out var Power))
					Power = 0;
				int[]? Duration = Utility.StringToIntArray(it["Dur"]);
				int Value_type = Convert.ToInt32(it["Value_type"]); // ForceValueType

				SkillValue skillValue = new(BForce_ID, Value, Duration, Value_type);
				SkillConfig.AddSkillValue(SkillIdx, skillValue);
			}
		}

		internal int CalculateCritDamage(Int32 criticalDamage)
		{
			switch (_skillInfoMain.Group)
			{
				case Enums.SkillGroup.SK_GROUP001:
				case Enums.SkillGroup.SK_GROUP002:
				case Enums.SkillGroup.SK_GROUP003:
				case Enums.SkillGroup.SK_GROUP004:
				case Enums.SkillGroup.SK_GROUP014:
				{
					criticalDamage += GetBaseCriticalDamage();
					return criticalDamage;
				}
				default:
				{
					return criticalDamage;
				}
			}
		}
	}
}
