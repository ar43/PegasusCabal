using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.Battle
{
	internal class BattleStats
	{
		public BattleStats(Int32 attack, Int32 magicAttack, Int32 swordSkillAmp, Int32 magicSkillAmp, Int32 attackRate, Int32 criticalRate, Int32 maxCriticalRate, int criticalDamage)
		{
			Attack = attack;
			MagicAttack = magicAttack;
			SwordSkillAmp = swordSkillAmp;
			MagicSkillAmp = magicSkillAmp;
			AttackRate = attackRate;
			CriticalRate = criticalRate;
			MaxCriticalRate = maxCriticalRate;
			CriticalDamage = criticalDamage;
		}

		public int Attack { get; private set; }
		public int MagicAttack { get; private set; }
		public int SwordSkillAmp { get; private set; }
		public int MagicSkillAmp { get; private set; }
		public int AttackRate { get; private set; }
		public int CriticalRate { get; private set; }
		public int MaxCriticalRate { get; private set; }
		public int CriticalDamage { get; private set; }
	}
}
