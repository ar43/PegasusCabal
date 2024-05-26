namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncLocation
	{
		public DbSyncLocation(Int32 x, Int32 y, Int32 worldId)
		{
			X = x;
			Y = y;
			WorldId = worldId;
		}

		public int X { get; private set; }
		public int Y { get; private set; }
		public int WorldId { get; private set; }
	}
}
