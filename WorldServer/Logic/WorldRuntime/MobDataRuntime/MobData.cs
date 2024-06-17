using WorldServer.Enums;
using WorldServer.Enums.Mob;

namespace WorldServer.Logic.WorldRuntime.MobDataRuntime
{
	internal class MobData
	{
		private const int FINDCNT4MOVELESS = 100000000;
		public MobData(Int32 id, Single moveSpeed, Single chasSpeed, Int32 property, MobPattern attkPattern, MobAggressive aggressive, String cooperate, String escape, MobAttack attack, Int32 scale, Int32 findCount, Int32 findInterval, Int32 moveInterval, Int32 chasInterval, Int32 alertRange, Int32 limt0Range, Int32 limt1Range, Int32 lEV, Int32 eXP, Int32 hP, Int32 defense, Int32 attacksR, Int32 defenseR, Int32 hPRechagR, Int32 interval1, Int32 phyAttMin1, Int32 phyAttMax1, Int32 reach1, Int32 range1, Int32 group1, Int32 stance1, Int32 interval2, Int32 phyAttMin2, Int32 phyAttMax2, Int32 reach2, Int32 range2, Int32 group2, Int32 stance2, Int32 boss, Int32 atkSignal, float radius, Int32 canatk)
		{
			Id = id;
			MoveSpeed = moveSpeed;
			ChasSpeed = chasSpeed;
			Property = property;
			AttkPattern = attkPattern;
			Aggressive = aggressive;
			Cooperate = cooperate;
			Escape = escape;
			Attack = attack;
			Scale = scale;
			FindCount = findCount;
			FindInterval = findInterval;
			MoveInterval = moveInterval;
			ChasInterval = chasInterval;
			AlertRange = alertRange;
			Limt0Range = limt0Range;
			Limt1Range = limt1Range;
			LEV = lEV;
			EXP = eXP;
			HP = hP;
			Defense = defense;
			AttacksR = attacksR;
			DefenseR = defenseR;
			HPRechagR = hPRechagR;
			Interval1 = interval1;
			PhyAttMin1 = phyAttMin1;
			PhyAttMax1 = phyAttMax1;
			Reach1 = reach1;
			Range1 = range1;
			Group1 = group1;
			Stance1 = stance1;
			Interval2 = interval2;
			PhyAttMin2 = phyAttMin2;
			PhyAttMax2 = phyAttMax2;
			Reach2 = reach2;
			Range2 = range2;
			Group2 = group2;
			Stance2 = stance2;
			Boss = boss;
			AtkSignal = atkSignal;
			Radius = radius;
			Canatk = canatk;

			IsMoveless = findCount >= FINDCNT4MOVELESS;

			DefaultSkill = new MobSkill(true, Interval1, PhyAttMin1, PhyAttMax1, Reach1, Range1, (SkillGroup)Group1, Stance1, Scale);
			SpecialSkill = new MobSkill(false, Interval2, PhyAttMin2, PhyAttMax2, Reach2, Range2, (SkillGroup)Group2, Stance2, Scale);
		}

		public int Id { get; private set; }
		public float MoveSpeed { get; private set; }
		public float ChasSpeed { get; private set; }
		public int Property { get; private set; }
		public MobPattern AttkPattern { get; private set; }
		public MobAggressive Aggressive { get; private set; }
		public string Cooperate { get; private set; }
		public string Escape { get; private set; }
		public MobAttack Attack { get; private set; }
		public int Scale { get; private set; }
		public int FindCount { get; private set; }
		public int FindInterval { get; private set; }
		public int MoveInterval { get; private set; }
		public int ChasInterval { get; private set; }
		public int AlertRange { get; private set; }
		public int Limt0Range { get; private set; }
		public int Limt1Range { get; private set; }
		public int LEV { get; private set; }
		public int EXP { get; private set; }
		public int HP { get; private set; }
		public int Defense { get; private set; }
		public int AttacksR { get; private set; }
		public int DefenseR { get; private set; }
		public int HPRechagR { get; private set; }
		private int Interval1 { get; set; }
		private int PhyAttMin1 { get; set; }
		private int PhyAttMax1 { get; set; }
		private int Reach1 { get; set; }
		private int Range1 { get; set; }
		private int Group1 { get; set; }
		private int Stance1 { get; set; }
		private int Interval2 { get; set; }
		private int PhyAttMin2 { get; set; }
		private int PhyAttMax2 { get; set; }
		private int Reach2 { get; set; }
		private int Range2 { get; set; }
		private int Group2 { get; set; }
		private int Stance2 { get; set; }
		public int Boss { get; private set; }
		public int AtkSignal { get; private set; }
		public float Radius { get; private set; }
		public int Canatk { get; private set; }
		public MobSkill DefaultSkill { get; private set; }
		public MobSkill SpecialSkill { get; private set; }
		public bool IsMoveless { get; private set; }
	}
}
