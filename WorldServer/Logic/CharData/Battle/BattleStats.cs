﻿namespace WorldServer.Logic.CharData.Battle
{
	internal class BattleStats
	{
		public BattleStats(Int32 attack, Int32 magicAttack, Int32 swordSkillAmp, Int32 magicSkillAmp, Int32 attackRate, Int32 criticalRate, Int32 maxCriticalRate, int criticalDamage, int defense, int defenseRate)
		{
			Attack = attack;
			MagicAttack = magicAttack;
			SwordSkillAmp = swordSkillAmp;
			MagicSkillAmp = magicSkillAmp;
			AttackRate = attackRate;
			CriticalRate = criticalRate;
			MaxCriticalRate = maxCriticalRate;
			CriticalDamage = criticalDamage;
			Defense = defense;
			DefenseRate = defenseRate;
		}

		public int Attack { get; private set; }
		public int MagicAttack { get; private set; }
		public int SwordSkillAmp { get; private set; }
		public int MagicSkillAmp { get; private set; }
		public int AttackRate { get; private set; }
		public int CriticalRate { get; private set; }
		public int MaxCriticalRate { get; private set; }
		public int CriticalDamage { get; private set; }
		public int Defense { get; private set; }
		public int DefenseRate { get; private set; }
	}
}
