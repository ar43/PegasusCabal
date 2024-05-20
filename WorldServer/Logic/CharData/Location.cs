using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.WorldRuntime;

namespace WorldServer.Logic.CharData
{
	internal class Location
	{
		

		public Instance? Instance;

		public MovementData Movement {get; private set;}

		public DBSyncPriority SyncPending { get; private set;}

		public Location(UInt16 x, UInt16 y)
		{
			Movement = new MovementData(false, x, y, 4.5f);
			SyncPending = DBSyncPriority.NONE;
		}

		public void Sync(DBSyncPriority prio)
		{
			if(prio > SyncPending)
				SyncPending = prio;
		}

		public DbSyncLocation GetDB()
		{
			//todo: check if X and Y are in WALL, reset pos
			//todo: check if current map resets X and Y on logout, reset pos
			if (Instance == null)
				throw new Exception("fixme");
			DbSyncLocation dbSyncLocation = new DbSyncLocation(Movement.X, Movement.Y, (Int32)Instance.MapId);
			return dbSyncLocation;
		}

		
	}
}
