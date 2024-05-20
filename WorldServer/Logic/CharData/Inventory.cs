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
		private Dictionary<UInt16, Item> _items;
		public UInt64 Alz { get; private set; }
		public DBSyncPriority SyncPending { get; private set; }

		public Inventory(InventoryData? protobuf, UInt64 alz)
		{
			_items = new Dictionary<UInt16, Item>();
			SyncPending = DBSyncPriority.NONE;

			if(protobuf != null )
			{
				foreach (var inv in protobuf.InventoryData_)
				{
					_items.Add((UInt16)inv.Key, new Item(inv.Value.Kind, inv.Value.Option, inv.Value.Serial, inv.Value.Duration));
				}
			}

			Alz = alz;
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
			foreach(var item in _items)
			{
				var slot = item.Key;
				var itemObj = item.Value;
				data.InventoryData_.Add(slot, item.Value.GetProtobuf());
			}
			return data;
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			foreach(var item in _items)
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

		public int Count()
		{
			return _items.Count;
		}
	}
}
