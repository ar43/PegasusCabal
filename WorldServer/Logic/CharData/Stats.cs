using System;
using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.CharData.Styles.Coefs;

namespace WorldServer.Logic.CharData
{
	internal class Stats
	{
		public Stats(UInt32 level, UInt32 exp, int str, int dex, int @int, UInt32 pnt, UInt32 rank)
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
		public int Str { get; private set; }
		public int Dex { get; private set; }
		public int Int { get; private set; }
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

		public int CalculateValueFromCoef(StatCoef coef)
		{
			return ((coef.STR * Str) + (coef.DEX * Dex) + (coef.INT * Int)) / 10000;
		}
	}
}
