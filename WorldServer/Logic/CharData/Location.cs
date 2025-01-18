using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.WorldRuntime.InstanceRuntime;

namespace WorldServer.Logic.CharData
{
	internal class Location
	{

		public FieldLocInfo DisconnectFieldInfo;
		public FieldLocInfo LastFieldLocInfo;
		public Instance? Instance;

		public MovementData Movement { get; private set; }

		public DBSyncPriority SyncPending { get; private set; }
		public PendingDungeon PendingDungeon { get; private set; }

		public Location(UInt16 x, UInt16 y)
		{
			Movement = new MovementData(x, y, 4.5f);
			SyncPending = DBSyncPriority.NONE;
			PendingDungeon = new();
		}

		public void Sync(DBSyncPriority prio)
		{
			if (prio > SyncPending)
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
			int x = Movement.X;
			int y = Movement.Y;

			if (Instance == null)
			{
				mapId = DisconnectFieldInfo.MapId;
				x = DisconnectFieldInfo.X; 
				y = DisconnectFieldInfo.Y;
			}
			else if (Instance.Type == InstanceType.DUNGEON)
			{
				mapId = LastFieldLocInfo.MapId;
				x = LastFieldLocInfo.X;
				y = LastFieldLocInfo.Y;
			}
			else
			{
				mapId = (Int32)Instance.MapId;
			}
				

			if (mapId == 0)
				throw new Exception("mapId == 0");
			DbSyncLocation dbSyncLocation = new DbSyncLocation(x, y, mapId);
			return dbSyncLocation;
		}


	}

	internal struct FieldLocInfo
	{
		public int X;
		public int Y;
		public int MapId;

		public FieldLocInfo(Int32 x, Int32 y, Int32 mapId)
		{
			X = x;
			Y = y;
			MapId = mapId;
		}
	}

	internal class PendingDungeon
	{
		public PendingDungeon()
		{
			DungeonInstanceId = 0;
			DungeonId = 0;
		}

		public ulong DungeonInstanceId { get; private set; }
		public int DungeonId { get; private set; }

		public void Clear()
		{
			DungeonInstanceId = 0; 
			DungeonId = 0;
		}
		public void Set(ulong dungeonInstanceId, int dungeonId)
		{
			DungeonInstanceId = dungeonInstanceId;
			DungeonId = dungeonId;
		}
	}
}
