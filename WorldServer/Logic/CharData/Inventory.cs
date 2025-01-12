using Shared.Protos;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Items;

namespace WorldServer.Logic.CharData
{
	internal class Inventory
	{
		private Dictionary<UInt16, Item> _items;
		private bool[] _occupiedSlots;
		private readonly int INV_SIZE = 64 * 4;
		public UInt64 Alz { get; private set; }
		public DBSyncPriority SyncPending { get; private set; }

		public Inventory(InventoryData? protobuf, UInt64 alz)
		{
			_items = new Dictionary<UInt16, Item>();
			SyncPending = DBSyncPriority.NONE;
			_occupiedSlots = new bool[INV_SIZE];

			if (protobuf != null)
			{
				foreach (var inv in protobuf.InventoryData_)
				{
					if (!AddItem((UInt16)inv.Key, new Item(inv.Value.Kind, inv.Value.Option, inv.Value.Serial, inv.Value.Duration)))
						throw new Exception("Error on inv init");
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

		public void GiveAlz(UInt64 amt)
		{
			if (amt < 0)
				throw new Exception("expected amt > 0");
			Alz += amt;
		}

		public InventoryData GetProtobuf()
		{
			InventoryData data = new InventoryData();
			foreach (var item in _items)
			{
				var slot = item.Key;
				data.InventoryData_.Add(slot, item.Value.GetProtobuf());
			}
			return data;
		}

		public bool UseItem(int fromSlot, bool sync = true)
		{
			if (fromSlot < 0 || fromSlot >= INV_SIZE)
				return false;
			if (!_items.TryGetValue((UInt16)fromSlot, out var item))
				return false;
			if(item.TryUse())
			{
				if(sync)
					Sync(DBSyncPriority.NORMAL);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ItemMove(int fromSlot, int toSlot, bool sync)
		{
			if (fromSlot < 0 || toSlot < 0)
				return false;
			if (fromSlot >= INV_SIZE || toSlot >= INV_SIZE)
				return false;

			if (!_items.TryGetValue((UInt16)fromSlot, out var item))
				return false;
			if (_items.ContainsKey((UInt16)toSlot))
				return false;
			_ = RemoveItem((UInt16)fromSlot);
			if (!AddItem((UInt16)toSlot, item))
				return false;

			if (sync)
				Sync(DBSyncPriority.NORMAL);
			return true;
		}

		public bool ItemSwap(int fromSlot1, int toSlot1, int fromSlot2, int toSlot2, bool sync)
		{
			if (fromSlot1 < 0 || toSlot1 < 0 || fromSlot2 < 0 || toSlot2 < 0)
				return false;
			if (fromSlot1 >= INV_SIZE || toSlot1 >= INV_SIZE || toSlot2 >= INV_SIZE || fromSlot2 >= INV_SIZE)
				return false;

			if (!_items.ContainsKey((UInt16)fromSlot2))
				return false;
			if (!_items.ContainsKey((UInt16)fromSlot1))
				return false;

			var savedItem = RemoveItem((UInt16)fromSlot2);
			if (savedItem == null)
				return false;
			if (!ItemMove(fromSlot1, toSlot1, false))
				return false;
			if (!AddItem((UInt16)toSlot2, savedItem))
				return false;

			if (sync)
				Sync(DBSyncPriority.NORMAL);
			return true;
		}

		private static UInt16 PosToSlot(int x, int y)
		{
			return (UInt16)(y * 8 + x);
		}

		public bool AddItem(UInt16 slot, Item item)
		{
			//todo, check item collision with a cache
			var slotX = slot % 8;
			var slotY = slot / 8;
			var itemWidth = item.GetWidth();
			var itemHeight = item.GetHeight();

			if (slotX > 8 - itemWidth)
				return false;
			if ((slotY % 8) > 8 - itemHeight)
				return false;
			for (int i = 0; i < itemWidth; i++)
			{
				for (int j = 0; j < itemHeight; j++)
				{
					if (_occupiedSlots[PosToSlot(slotX + i, slotY + j)])
						return false;
				}
			}

			_items.Add((UInt16)slot, item);

			for (int i = 0; i < itemWidth; i++)
			{
				for (int j = 0; j < itemHeight; j++)
				{
					_occupiedSlots[PosToSlot(slotX + i, slotY + j)] = true;
				}
			}
			return true;
		}

		public UInt32 RemoveAllQuestItemsByKind(uint kind)
		{
			UInt32 cnt = 0;
			List<UInt16> slots = new();
			foreach(var item in _items)
			{
				if (item.Value.Kind == kind)
					slots.Add(item.Key);
			}
			foreach(var slot in slots)
			{
				var item = RemoveItem(slot);
				if (item != null)
					cnt += item.GetQuestItemCount();
			}
			return cnt;
		}

		public Item? RemoveItem(UInt16 slot)
		{
			//todo, check item collision with a cache

			if (!_items.ContainsKey(slot))
				return null;

			var slotX = slot % 8;
			var slotY = slot / 8;
			var item = _items[slot];

			_items.Remove((UInt16)slot);

			for (int i = 0; i < item.GetWidth(); i++)
			{
				for (int j = 0; j < item.GetHeight(); j++)
				{
					_occupiedSlots[PosToSlot(slotX + i, slotY + j)] = false;
				}
			}
			return item;
		}

		public Item? PeekItem(UInt16 slot)
		{
			if (!_items.ContainsKey(slot))
				return null;

			var item = _items[slot];
			return item;
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			foreach (var item in _items)
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

		public void DebugWipe()
		{
			_items = new();
		}
	}
}
