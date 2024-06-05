using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.Battle
{
	internal class BattleStats
	{
		public BattleStats(Int32 attack, Int32 magicAttack, Int32 swordSkillAmp, Int32 magicSkillAmp)
		{
			Attack = attack;
			MagicAttack = magicAttack;
			SwordSkillAmp = swordSkillAmp;
			MagicSkillAmp = magicSkillAmp;
		}

		public int Attack { get; private set; }
		public int MagicAttack { get; private set; }
		public int SwordSkillAmp { get; private set; }
		public int MagicSkillAmp { get; private set; }
	}
}
