using WorldServer.Logic.CharData.Items;

namespace WorldServer.Logic.AccountData
{
	internal class CashInventory
	{
		public static readonly int MAX_ITEMS = 128;
		private List<Item> _items;

		public CashInventory()
		{
			_items = new List<Item>();
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			var itemCount = Math.Min(MAX_ITEMS, _items.Count);
			for (UInt16 i = 0; i < itemCount; i++)
			{
				var item = _items[i];
				if (item != null)
				{
					bytes.AddRange(BitConverter.GetBytes(item.Kind));
					bytes.AddRange(BitConverter.GetBytes(item.Serial));
					bytes.AddRange(BitConverter.GetBytes(item.Option));
					bytes.AddRange(BitConverter.GetBytes(i));
					bytes.AddRange(BitConverter.GetBytes(item.Duration));
				}
			}
			return bytes.ToArray();
		}

		public int Count()
		{
			return _items.Count();
		}
	}
}
