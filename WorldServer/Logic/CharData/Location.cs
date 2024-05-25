using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.WorldRuntime.InstanceRuntime;

namespace WorldServer.Logic.CharData
{
    internal class Location
	{

		public int LastMapId { private get; set; }
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
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public DbSyncLocation GetDB()
		{
			//todo: check if X and Y are in WALL, reset pos
			//todo: check if current map resets X and Y on logout, reset pos
			//todo: check if in first 4 map towns, reset pos (weird ep8 behaviour)
			int mapId;
			if (Instance == null)
				mapId = LastMapId;
			else
				mapId = (Int32)Instance.MapId;
			if (mapId == 0)
				throw new Exception("mapId == 0");
			DbSyncLocation dbSyncLocation = new DbSyncLocation(Movement.X, Movement.Y, mapId);
			return dbSyncLocation;
		}

		
	}
}
