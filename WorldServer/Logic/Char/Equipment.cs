using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Logic.Char
{
	internal class Equipment
	{
		public Item[] List = new Item[(Int32)EquipmentIndex.NUM_EQUIPMENT];

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			for(int i = 0; i < List.Length; i++)
			{
				var item = List[i];
				if(item != null && item.Kind != 0)
				{
					bytes.AddRange(BitConverter.GetBytes(item.Kind));
					bytes.AddRange(BitConverter.GetBytes(item.Serial));
					bytes.AddRange(BitConverter.GetBytes(item.Option));
					bytes.AddRange(BitConverter.GetBytes((UInt16)i));
					bytes.AddRange(BitConverter.GetBytes(item.Duration));
				}
			}
			return bytes.ToArray();
		}

		public UInt16 Count()
		{
			UInt16 count = 0;
			for (int i = 0; i < List.Length; i++)
			{
				var item = List[i];
				if (item != null && item.Kind != 0)
				{
					count++;
				}
			}
			return count;
		}
	}
}
