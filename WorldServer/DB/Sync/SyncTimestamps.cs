namespace WorldServer.DB.Sync
{
	internal class SyncTimestamps
	{
		public DateTime Equipment;
		public DateTime Inventory;
		public DateTime Location;
		public DateTime QuickSlotBar;
		public DateTime Skills;
		public DateTime Stats;
		public DateTime Status;
		public DateTime Quest;
		public DateTime Style;

		public SyncTimestamps()
		{
			Equipment = DateTime.MinValue;
			Inventory = DateTime.MinValue;
			Location = DateTime.MinValue;
			QuickSlotBar = DateTime.MinValue;
			Skills = DateTime.MinValue;
			Stats = DateTime.MinValue;
			Status = DateTime.MinValue;
			Quest = DateTime.MinValue;
			Style = DateTime.MinValue;
		}
	}
}
