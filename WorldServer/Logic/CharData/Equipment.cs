using Shared.Protos;
using System.Reflection.Metadata;
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

		public Item? GetItem(EquipmentIndex index)
		{
			var item = _list[(int)index];
			if (item != null && item.Kind == 0)
				return null;
			return item;
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
							stats.AttackRate += item.GetAttackRate();
							break;
						}
						case EquipmentIndex.RING1:
						case EquipmentIndex.RING2:
						case EquipmentIndex.RING3:
						case EquipmentIndex.RING4:
						{
							//TEMP
							//TODO: PROPER OPTION READING SYSTEM UNIVERSAL
							if(item.GetOpt1() == 8)
							{
								stats.CriticalDamage += item.GetOpt1Val();
							}
							else if (item.GetOpt1() == 9)
							{
								stats.CriticalRate += item.GetOpt1Val();
							}
							break;
						}
						case EquipmentIndex.GLOVES:
						case EquipmentIndex.SUIT:
						case EquipmentIndex.BOOTS:
						case EquipmentIndex.HELMET:
						{
							stats.Defense += item.GetDefense();
							stats.DefenseRate += item.GetDefenseRate();
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
