using WorldServer.Enums;
using WorldServer.Logic.CharData.Skills;

namespace WorldServer.Logic.CharData
{
	internal class BuffManager
	{
		public BuffManager()
		{
			BuffFlag = new(0);
			_activeBuffs = new();
		}

		public void ActivateBuff(Skill skill)
		{
			var list = skill.GetSkillBuffData();
			if (list == null)
			{
				return;
			}

			_activeBuffs[skill.Id] = list;
		}

		public void RemoveBuff(int skillId)
		{
			_activeBuffs.Remove(skillId);
		}

		public BuffStats GetBuffStats()
		{
			BuffStats stats = new();

			foreach (var buffList in _activeBuffs.Values)
			{
				foreach (var buff in buffList)
				{
					stats.AddBuff(buff);
				}
			}

			return stats;
		}

		public BuffFlag BuffFlag { get; private set; }
		private Dictionary<int, List<BuffData>> _activeBuffs;
	}

	internal class BuffStats
	{

		public int Attack;
		public int MagicAttack;
		public int SwordSkillAmp = 0;
		public int MagicSkillAmp = 0;
		public int AttackRate = 0;
		public int CriticalRate = 0;
		public int CriticalDamage = 0;
		public int Defense = 0;
		public int DefenseRate = 0;

		public BuffStats()
		{
		}

		public void AddBuff(BuffData buff)
		{
			switch (buff.ForceCode)
			{
				case ForceCode.SAT_ATTACK:
				{
					Attack += buff.BuffValue;
					break;
				}
				case ForceCode.SAT_MATTACK:
				{
					MagicAttack += buff.BuffValue;
					break;
				}
				case ForceCode.SAT_DEFENSE:
				{
					Defense += buff.BuffValue;
					break;
				}
				case ForceCode.SAT_ARATE:
				{
					AttackRate += buff.BuffValue;
					break;
				}
				case ForceCode.SAT_DRATE:
				{
					DefenseRate += buff.BuffValue;
					break;
				}
				case ForceCode.SAT_CRIDMG:
				{
					CriticalDamage += buff.BuffValue;
					break;
				}
				case ForceCode.SAT_CRIRATE:
				{
					CriticalRate += buff.BuffValue;
					break;
				}
				case ForceCode.SAT_REACH_INC:
				{
					break;
				}
				default:
					throw new NotImplementedException();
			}
		}
	}

	internal class BuffData
	{
		public ForceCode ForceCode { get; private set; }
		public int BuffValue { get; private set; }

		public BuffData(ForceCode forceCode, Int32 buffValue)
		{
			ForceCode = forceCode;
			BuffValue = buffValue;
		}
	}

	internal class BuffFlag
	{
		private UInt32 _value;

		public BuffFlag(UInt32 value)
		{
			_value = value;
		}

		public UInt32 Serialize()
		{
			return _value;
		}

		public void Set(UInt32 value)
		{
			_value = value;
		}
	}
}
