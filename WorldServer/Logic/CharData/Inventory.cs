using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Logic.CharData
{
	internal class Inventory
	{
		public Dictionary<UInt16, Item> Items; //make private
		public UInt64 Alz; //make private
		public DBSyncPriority SyncPending { get; private set; }

		public Inventory()
		{
			Items = new Dictionary<UInt16, Item>();
			SyncPending = DBSyncPriority.NONE;
		}
		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public InventoryData GetProtobuf()
		{
			InventoryData data = new InventoryData();
			foreach(var item in Items)
			{
				var slot = item.Key;
				var itemObj = item.Value;
				data.InventoryData_.Add(slot, new InventoryData.Types.InventoryDataItem { Kind = itemObj.Kind, Option = itemObj.Option }); //todo: add GetProtobuf to item and unify the object
			}
			return data;
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			foreach(var item in Items)
			{
				if (item.Value != null && item.Value.Kind != 0)
				{
					bytes.AddRange(BitConverter.GetBytes(item.Value.Kind));
					bytes.AddRange(BitConverter.GetBytes(item.Value.Serial));
					bytes.AddRange(BitConverter.GetBytes(item.Value.Option));
					bytes.AddRange(BitConverter.GetBytes(item.Key));
					bytes.AddRange(BitConverter.GetBytes(item.Value.Duration));
				}
			}
			return bytes.ToArray();
		}
	}
}
