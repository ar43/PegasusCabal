using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;

namespace WorldServer.Logic.CharData
{
	internal class Stats
	{
		public Stats(UInt32 level, UInt32 exp, UInt32 str, UInt32 dex, UInt32 @int, UInt32 pnt, UInt32 rank)
		{
			Level = level;
			Exp = exp;
			Str = str;
			Dex = dex;
			Int = @int;
			Pnt = pnt;
			Rank = rank;
			Axp = 0;
			SyncPending = DBSyncPriority.NONE;
		}

		public DBSyncPriority SyncPending { get; private set; }
		public UInt32 Level { get; private set; }
		public UInt32 Exp { get; private set; }
		public UInt32 Axp { get; private set; }
		public UInt32 Str { get; private set; }
		public UInt32 Dex { get; private set; }
		public UInt32 Int { get; private set; }
		public UInt32 Pnt { get; private set; }
		public UInt32 Rank { get; private set; }
		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public DbSyncStats GetDB()
		{
			DbSyncStats stats = new DbSyncStats((Int32)Level, Exp, (Int32)Axp, (Int32)Str, (Int32)Dex, (Int32)Int, (Int32)Pnt, (Int32)Rank);
			return stats;
		}
	}
}
