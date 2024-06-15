using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.InstanceRuntime;

namespace WorldServer.Logic.WorldRuntime.MobDataRuntime
{
	internal class MobSkill
	{
		public MobSkill(bool isDefSkill, Int32 interval, Int32 phyAttMin, Int32 phyAttMax, Int32 reach, Int32 range, SkillGroup skillGroup, Int32 stance, int scale)
		{
			IsDefSkill = isDefSkill;
			Interval = interval;
			PhyAttMin = phyAttMin;
			PhyAttMax = phyAttMax;
			Reach = reach;
			Range = range;
			SkillGroup = skillGroup;
			Stance = stance;
			PhyAttDff = PhyAttMax - PhyAttMin;
			N2 = (scale + 1) >> 2;
			ValidDist = (scale >> 1) + Reach;
			ValidOffset = N2 + ((Reach - N2) >> 1);
			if(ValidOffset < N2)
				ValidOffset = N2;
		}

		public bool IsDefSkill { get; private set; }
		public int Interval { get; private set; }
		public int PhyAttMin { get; private set; }
		public int PhyAttMax { get; private set; }
		public int PhyAttDff { get; private set; }
		public int Reach {  get; private set; }
		public int Range { get; private set; }
		public SkillGroup SkillGroup { get; private set; }
		public int Stance { get; private set; }
		public int N2 { get; private set; }
		public int ValidDist { get; private set; }
		public int ValidOffset { get; private set; }

		public static bool IsValid(int MobsX, int MobsY, int TargetPos, Instance instance)
		{
			int TargetX = ((TargetPos) & 0xFFFF);
			int TargetY = ((TargetPos) >> 16);

			int iDx = TargetX - MobsX;
			int iDy = TargetY - MobsY;

			float fDx = (float)(iDx);
			float fDz = (float)(iDy);

			int iAbsX = Math.Abs(iDx);
			int iAbsZ = Math.Abs(iDy);

			int iSignX = (iDx == 0 ? 0 : iDx / iAbsX);
			int iSignZ = (iDy == 0 ? 0 : iDy / iAbsZ);

			float fCurDistance = (Single)Math.Sqrt((float)(iDx * iDx + iDy * iDy));
			int iCurDistance = (int)(fCurDistance);

			if (iCurDistance >= 2)
			{
				int iCheckX = 0;
				int iCheckY = 0;
				int iOldCheckX = 0;
				int iOldCheckY = 0;

				for (int iCheckDistance = 1; iCheckDistance < iCurDistance; iCheckDistance++)
				{
					if (iCheckDistance > 1)
					{
						iOldCheckX = iCheckX;
						iOldCheckY = iCheckY;
					}

					//=================================================================================================
					iCheckX = (iDx == 0 ? MobsX : (MobsX + (int)(iCheckDistance / fCurDistance * fDx + 0.5f * iSignX)));
					iCheckY = (iDy == 0 ? MobsY : (MobsY + (int)(iCheckDistance / fCurDistance * fDz + 0.5f * iSignZ)));

					if (iCheckDistance > 1)
					{
						int iDifX = Math.Abs(iCheckX) - Math.Abs(iOldCheckX);
						if (Math.Abs(iDifX) > 1)
						{
							iCheckX = iCheckX - iSignX;
						}

						int iDifZ = Math.Abs(iCheckY) - Math.Abs(iOldCheckY);
						if (Math.Abs(iDifZ) > 1)
						{
							iCheckY = iCheckY - iSignZ;
						}

					}

					if (instance.CheckTileOverAttack((UInt16)iCheckX, (UInt16)iCheckY))
					{
						return true;
					}
					else if(instance.CheckTileUnmovable((UInt16)iCheckX, (UInt16)iCheckY))
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
