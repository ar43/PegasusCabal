using Shared.Protos;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Battle;
using WorldServer.Logic.CharData.Items;

namespace WorldServer.Logic.CharData
{
	internal class Equipment
	{
		public DBSyncPriority SyncPending { get; private set; }
		private Item[] _list;

		public Equipment(EquipmentData? protobuf)
		{
			SyncPending = DBSyncPriority.NONE;
			_list = new Item[(Int32)EquipmentIndex.NUM_EQUIPMENT];

			if (protobuf != null)
			{
				foreach (var eq in protobuf.EquipmentData_)
				{
					Item item = new Item(eq.Value.Kind, eq.Value.Option, eq.Value.Serial, eq.Value.Duration);
					_list[(Int32)eq.Key] = item;
				}
			}

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

			for (uint i = 0; i < _list.Length; i++)
			{
				var item = _list[i];
				if (item == null || item.Kind == 0)
					continue;
				data.EquipmentData_.Add(i, item.GetProtobuf());
			}

			return data;
		}

		public EquStats GetStats()
		{
			EquStats stats = new EquStats();
			for(int i = 0; i < _list.Length; i++)
			{
				var item = _list[i];
				if(item != null && item.Kind != 0)
				{
					switch((EquipmentIndex)i)
					{
						case EquipmentIndex.RIGHTHAND:
						case EquipmentIndex.LEFTHAND:
						{
							stats.Attack += item.GetAttack();
							stats.MagicAttack += item.GetMagicAttack();
							break;
						}
						default:
						{
							//TODO
							break;
						}
					}
				}
			}
			return stats;
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			for (int i = 0; i < _list.Length; i++)
			{
				var item = _list[i];
				if (item != null && item.Kind != 0)
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
			for (int i = 0; i < _list.Length; i++)
			{
				var item = _list[i];
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
			for (int i = 0; i < _list.Length; i++)
			{
				var item = _list[i];
				if (item != null && item.Kind != 0)
				{
					count++;
				}
			}
			return count;
		}
	}
}
