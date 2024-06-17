using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;

namespace WorldServer.Logic.CharData
{
	internal class Status
	{
		public Status(int hp, int maxHp, int mp, int maxMp, int sp, int maxSP)
		{
			Hp = hp;
			MaxHp = maxHp;
			Mp = mp;
			MaxMp = maxMp;
			Sp = sp;
			MaxSp = maxSP;
			SyncPending = DBSyncPriority.NONE;
			IsDead = false;

			DebugUndying = true;
		}

		public DBSyncPriority SyncPending { get; private set; }
		public int Hp { get; private set; }
		public int MaxHp { get; private set; }
		public int Mp { get; private set; }
		public int MaxMp { get; private set; }
		public int Sp { get; private set; }
		public int MaxSp { get; private set; }

		public bool IsDead { get; private set; }

		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public void TakeHp(int damage)
		{
			Hp -= damage;
			if (Hp <= 0)
				Hp = 0;

			if(DebugUndying)
				Hp = Math.Max(Hp, 1);
		}

		public DbSyncStatus GetDB()
		{
			return new DbSyncStatus((Int32)Hp, (Int32)MaxHp, (Int32)Mp, (Int32)MaxMp, (Int32)Sp, (Int32)MaxSp);
		}

		//////////////////////////////////////////////////////////////////////////////
		// DEBUG

		public bool DebugUndying { get; private set; }
	}
}
