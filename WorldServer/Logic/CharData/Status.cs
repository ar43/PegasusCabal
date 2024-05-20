using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;

namespace WorldServer.Logic.CharData
{
	internal class Status
	{
		public Status(UInt32 hp, UInt32 maxHp, UInt32 mp, UInt32 maxMp, UInt32 sp, UInt32 maxSP)
		{
			Hp = hp;
			MaxHp = maxHp;
			Mp = mp;
			MaxMp = maxMp;
			Sp = sp;
			MaxSp = maxSP;
			SyncPending = DBSyncPriority.NONE;
		}

		public DBSyncPriority SyncPending { get; private set; }
		public UInt32 Hp { get; private set; }
		public UInt32 MaxHp { get; private set; }
		public UInt32 Mp { get; private set; }
		public UInt32 MaxMp { get; private set; }
		public UInt32 Sp { get; private set; }
		public UInt32 MaxSp { get; private set; }

		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
		}

		public DbSyncStatus GetDB()
		{
			return new DbSyncStatus((Int32)Hp, (Int32)MaxHp, (Int32)Mp, (Int32)MaxMp, (Int32)Sp, (Int32)MaxSp);
		}
	}
}
