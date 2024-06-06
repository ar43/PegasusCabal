using LibPegasus.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;

namespace WorldServer.Logic.CharData.Skills
{
	internal class SkillInfo
	{
		public SkillInfo()
		{
			MainData = new();
		}

		public Dictionary<int, SkillInfoMain> MainData { get; private set; }

		public void Add(int id, SkillInfoMain mainInfo)
		{
			MainData.Add(id, mainInfo);
		}
	}

	internal class SkillInfoMain
	{
		public SkillInfoMain(Int32 skillIdx, Int32 multi, Int32 target, Int32 max_Target, Int32 type, Int32 group, Int32[]? dur, Int32 bFX_Dur, Int32 value_Ref, Int32 bfx_Ref, Int32 property, Int32 element, Int32 skillExp1, Int32 skillExp2, Int32 grip, Int32 exclusive, Int32 useCase, Int32 reach, Int32 range, Int32 rangeType, Int32 firingFrame, Int32 hitFrame, Int32 move_Method, SkillCoef attackCoef, Int32[]? aRatingCoef, Int32[]? criticalRateCoef, Int32[]? criticalMultiCoef, Int32[]? mpCoef, Int32[]? mPWasteBModeCoef, Int32 sPWasteVal, Int32 checklimitNormal, Int32 checklimitCombo, Int32 upkeepctime, Int32 rate, Int32 ampCount, Int32 damageType)
		{
			SkillIdx = skillIdx;
			Multi = multi;
			Target = target;
			Max_Target = max_Target;
			Type = (SkillType)type;
			Group = (SkillGroup)group;
			Dur = dur;
			BFX_Dur = bFX_Dur;
			Value_Ref = value_Ref;
			Bfx_Ref = bfx_Ref;
			Property = property;
			Element = element;
			SkillExp1 = skillExp1;
			SkillExp2 = skillExp2;
			Grip = grip;
			Exclusive = exclusive;
			UseCase = useCase;
			Reach = reach;
			Range = range;
			RangeType = rangeType;
			FiringFrame = firingFrame;
			HitFrame = hitFrame;
			Move_Method = move_Method;
			AttackCoef = attackCoef;
			ARatingCoef = new SkillCoef(aRatingCoef);
			CriticalRateCoef = new SkillCoef(criticalRateCoef);
			CriticalMultiCoef = new SkillCoef(criticalMultiCoef);
			MpCoef = mpCoef;
			MPWasteBModeCoef = mPWasteBModeCoef;
			SPWasteVal = sPWasteVal;
			ChecklimitNormal = checklimitNormal;
			ChecklimitCombo = checklimitCombo;
			Upkeepctime = upkeepctime;
			Rate = rate;
			AmpCount = ampCount;
			DamageType = damageType;
		}

		public int SkillIdx { get; private set; }
		public int Multi { get; private set; }
		public int Target { get; private set; }
		public int Max_Target { get; private set; }
		public SkillType Type { get; private set; }
		public SkillGroup Group { get; private set; }
		public int[]? Dur { get; private set; }
		public int BFX_Dur { get; private set; }
		public int Value_Ref { get; private set; }
		public int Bfx_Ref { get; private set; }
		public int Property { get; private set; }
		public int Element { get; private set; }
		public int SkillExp1 { get; private set; }
		public int SkillExp2 { get; private set; }
		public int Grip { get; private set; }
		public int Exclusive { get; private set; }
		public int UseCase { get; private set; }
		public int Reach { get; private set; }
		public int Range { get; private set; }
		public int RangeType { get; private set; }
		public int FiringFrame { get; private set; }
		public int HitFrame { get; private set; }
		public int Move_Method { get; private set; }
		public SkillCoef? AttackCoef { get; private set; }
		public SkillCoef? ARatingCoef { get; private set; }
		public SkillCoef? CriticalRateCoef { get; private set; }
		public SkillCoef? CriticalMultiCoef { get; private set; }
		public int[]? MpCoef { get; private set; }
		public int[]? MPWasteBModeCoef { get; private set; }
		public int SPWasteVal { get; private set; }
		public int ChecklimitNormal { get; private set; }
		public int ChecklimitCombo { get; private set; }
		public int Upkeepctime { get; private set; }
		public int Rate { get; private set; }
		public int AmpCount { get; private set; }
		public int DamageType { get; private set; }
	}

	internal class SkillCoef
	{
		public SkillCoef(int[]? array)
		{
			if(array == null)
			{
				CoefA = 0;
				CoefB = 0;
				CoefC = 0;
				CoefD = 0;
			}
			else
			{
				if(array.Length > 0)
					CoefA = array[0];
				if (array.Length > 1)
					CoefB = array[1];
				if (array.Length > 2)
					CoefC = array[2];
				if (array.Length > 3)
					CoefD = array[3];
			}

		}

		public int CoefA { get; private set; }
		public int CoefB { get; private set; }
		public int CoefC { get; private set; }
		public int CoefD { get; private set; }
	}
}
