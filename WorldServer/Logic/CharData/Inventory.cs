using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal class Inventory
	{
		public Dictionary<UInt16, Item> Items;
		public UInt64 Alz;

		public Inventory()
		{
			Items = new Dictionary<UInt16, Item>();
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
