using LibPegasus.JSON;
using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using static Shared.Protos.EquipmentData.Types;

namespace WorldServer.Logic.CharData
{
	internal class Equipment
	{
		public DBSyncPriority SyncPending { get; private set; }
		public Item[] List; //todo: set private

		public Equipment()
		{
			SyncPending = DBSyncPriority.NONE;
			List = new Item[(Int32)EquipmentIndex.NUM_EQUIPMENT];
		}
		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public EquipmentData GetProtobuf()
		{
			EquipmentData data = new EquipmentData();

			for(uint i = 0; i < List.Length; i++)
			{
				var item = List[i];
				if (item == null || item.Kind == 0)
					continue;
				EquipmentDataItem protoItem = new EquipmentDataItem { Option = item.Option, Kind = item.Kind };
				data.EquipmentData_.Add(i, protoItem);
			}

			return data;
		}

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

		public byte[] SerializeEx()
		{
			var bytes = new List<byte>();
			for (int i = 0; i < List.Length; i++)
			{
				var item = List[i];
				if (item != null && item.Kind != 0)
				{
					bytes.Add((byte)i);
					bytes.AddRange(BitConverter.GetBytes(item.Kind));
					bytes.AddRange(BitConverter.GetBytes(item.Option));
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
