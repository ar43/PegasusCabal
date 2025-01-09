using WorldServer.Enums;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncRequest
	{
		public DbSyncRequest(DBSyncPriority prio, int charId, Boolean final)
		{
			Timestamp = DateTime.UtcNow;
			Priority = prio;
			CharId = charId;
			if (prio == DBSyncPriority.NONE || prio >= DBSyncPriority.NUM_PRIORITIES)
				throw new Exception("invalid priority");
			Final = final;
		}

		public DbSyncEquipment? DbSyncEquipment { get; set; }
		public DbSyncInventory? DbSyncInventory { get; set; }
		public DbSyncLocation? DbSyncLocation { get; set; }
		public DbSyncQuickSlotBar? DbSyncQuickSlotBar { get; set; }
		public DbSyncSkills? DbSyncSkills { get; set; }
		public DbSyncStats? DbSyncStats { get; set; }
		public DbSyncStatus? DbSyncStatus { get; set; }
		public DbSyncQuest? DbSyncQuest { get; set; }
		public DateTime Timestamp { get; private set; }
		public DBSyncPriority Priority { get; private set; }
		public int CharId { get; private set; }
		public bool Final { get; private set; }

	}
}
