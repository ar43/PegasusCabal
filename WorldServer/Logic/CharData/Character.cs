using WorldServer.Enums;
using WorldServer.Logic.CharData.Battle;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.CharData.Styles;
using WorldServer.Logic.SharedData;

namespace WorldServer.Logic.CharData
{
    internal class Character
	{
		public Character(Style style, string name) //used just in connection, not actually fr
		{
			Style = style;
			Name = name;
			Location = new Location(0, 0);
			LiveStyle = new LiveStyle(0);
			BuffFlag = new BuffFlag(0);
			ActionFlag = new ActionFlag(0);
			Id = 0;
		}

		public Character(Style style, String name, Equipment? equipment, Inventory? inventory, LearnedSkills? skills, QuickSlotBar? quickSlotBar, Location location, Stats? stats, Status? status, int id, int nation)
		{
			Equipment = equipment;
			Inventory = inventory;
			Skills = skills;
			QuickSlotBar = quickSlotBar;
			Location = location;
			Stats = stats;
			Status = status;
			Style = style;
			Name = name;
			LiveStyle = new LiveStyle(0);
			BuffFlag = new BuffFlag(0);
			ActionFlag = new ActionFlag(0);
			Id = id;
			Nation = (NationCode)nation;
		}

		public bool Verify()
		{
			//TODO
			return Style.Verify();
		}

		public Style Style { get; set; }
		public string Name { get; set; }
		public int Id { get; private set; }
		public NationCode Nation { get; private set; }
		public ObjectIndexData? ObjectIndexData { get; set; }
		public Equipment? Equipment { get; set; }
		public Inventory? Inventory { get; set; }
		public LearnedSkills? Skills { get; set; }
		public QuickSlotBar? QuickSlotBar { get; set; }
		public Location Location { get; private set; }
		public Stats? Stats { get; set; }
		public Status? Status { get; set; }

		public LiveStyle LiveStyle { get; private set; }
		public ActionFlag ActionFlag { get; private set; }
		public BuffFlag BuffFlag { get; private set; }
		public DBSyncPriority SyncPending { get; private set; }
		public bool UninitOnSync { get; private set; }


		public void Sync(DBSyncPriority prio, bool uninitOnSync = false)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
			if (uninitOnSync)
				UninitOnSync = uninitOnSync;
		}

		public void ClearSync()
		{
			SyncPending = DBSyncPriority.NONE;
			Location.Sync(DBSyncPriority.NONE);
			Equipment.Sync(DBSyncPriority.NONE);
			Inventory.Sync(DBSyncPriority.NONE);
			Status.Sync(DBSyncPriority.NONE);
			Stats.Sync(DBSyncPriority.NONE);
			Skills.Sync(DBSyncPriority.NONE);
			QuickSlotBar.Sync(DBSyncPriority.NONE);
		}

		public BattleStats CalculateBattleStats()
		{
			var equStats = Equipment.GetStats();

			int attack = 0;
			attack += equStats.Attack; //attack from equipment
			//Serilog.Log.Debug($"attack1: {attack}");
			attack += Stats.CalculateValueFromCoef(Style.BattleStyle.StatMaxAtt); //attack from stats
			//Serilog.Log.Debug($"attack2: {attack}");
			attack += Style.CalculateValueFromCoef(Style.BattleStyle.AttackCoef); //attack from battle style level
			//Serilog.Log.Debug($"attack3: {attack}");

			int magicAttack = 0;
			magicAttack += equStats.MagicAttack;
			magicAttack += Stats.CalculateValueFromCoef(Style.BattleStyle.StatMagAtt);
			magicAttack += Style.CalculateValueFromCoef(Style.BattleStyle.MagAttCoef);

			int attackRate = 0;
			attackRate += equStats.AttackRate;
			attackRate += Stats.CalculateValueFromCoef(Style.BattleStyle.StatAttckR);
			attackRate += Style.CalculateValueFromCoef(Style.BattleStyle.AttckRCoef);

			int maxCriticalRate = Stats.MAX_CRITICAL_RATE;

			int criticalRate = 0;
			criticalRate += Stats.BASE_CR;
			criticalRate += equStats.CriticalRate;
			criticalRate = Math.Min(criticalRate, maxCriticalRate);

			int criticalDamage = 0;
			criticalDamage += Stats.BASE_CD;
			criticalDamage += equStats.CriticalDamage;

			int swordSkillAmp = equStats.SwordSkillAmp;
			int magicSkillAmp = equStats.MagicSkillAmp;

			Serilog.Log.Debug($"CalculateBattleStats: attack: {attack} magic attack: {magicAttack} attack rate: {attackRate} critical rate: {criticalRate} crit dmg: {criticalDamage}");

			return new BattleStats(attack, magicAttack, swordSkillAmp, magicSkillAmp, attackRate, criticalRate, maxCriticalRate, criticalDamage);
		}

	}
}
