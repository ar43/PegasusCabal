using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Utils
{
	public static class BattleFormula
	{
		public static int GetHR(int iAttackR, int iDefenseR)
		{
			if (iAttackR + iDefenseR == 0)
			{
				throw new DivideByZeroException("iAttackR + iDefenseR == 0");
			}
			else
			{
				return (int)(50 * iAttackR / (iAttackR + iDefenseR) + 50);
			}
		}

		public static int AdjustHR(int var, int mn, int mx)
		{
			if ((var) < (mn))
				(var) = (mn);
			else if ((var) > (mx))
				(var) = (mx);
			return var;
		}

		public static int GetEXP(int iDmg, int iHpMax, int iMobEXP, int LvDiff, int ExpM )
		{
			return (int)((((iDmg) * (1000 - (LvDiff)) / (iHpMax)) * (iMobEXP) / 1000)) * ExpM;
		}

		public static int GetLvlDiff(int AttackerLEV, int DefenderLEV)
		{
			return (int)(((AttackerLEV) - (DefenderLEV)) * ((AttackerLEV) + 30) * 1000 / (22 * (AttackerLEV) + 102));
		}
	}
}
