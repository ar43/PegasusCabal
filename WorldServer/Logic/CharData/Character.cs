using WorldServer.Enums;
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

		public Character(Style style, String name, Equipment? equipment, Inventory? inventory, Skills? skills, QuickSlotBar? quickSlotBar, Location location, Stats? stats, Status? status, int id, int nation)
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
		public Skills? Skills { get; set; }
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

	}
}
