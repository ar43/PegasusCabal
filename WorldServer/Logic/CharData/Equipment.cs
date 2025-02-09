using Shared.Protos;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Battle;
using WorldServer.Logic.CharData.Items;

namespace WorldServer.Logic.CharData
{
	internal class Equipment
	{
		public DBSyncPriority SyncPending { get; private set; }
		private Item?[] _list;

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

		public bool CheckItemTypeForSlot(Character chr, Item item, UInt16 slot)
		{
			EquipmentIndex ei = (EquipmentIndex)slot;
			var type = item.GetItemType();

			switch (ei)
			{
				case EquipmentIndex.HELMET:
				{
					return type == ItemType.IDT_HMET;
				}
				case EquipmentIndex.SUIT:
				{
					return type == ItemType.IDT_SUIT;
				}
				case EquipmentIndex.GLOVES:
				{
					// Handle gloves
					return type == ItemType.IDT_GLOV;
				}
				case EquipmentIndex.BOOTS:
				{
					// Handle boots
					return type == ItemType.IDT_BOOT;
				}
				case EquipmentIndex.RIGHTHAND:
				case EquipmentIndex.LEFTHAND:
				{
					if (type == ItemType.IDT_LKWP)
					{
						if (chr.Style.BattleStyleNum == 3 || chr.Style.BattleStyleNum == 4)
						{
							type = ItemType.IDT_MWPN;
						}
						else if (chr.Style.BattleStyleNum == 2)
						{
							type = ItemType.IDT_2HND;
						}
						else
						{
							type = ItemType.IDT_1HND;
						}
					}

					EquipmentIndex otherSide = ei == EquipmentIndex.RIGHTHAND ? EquipmentIndex.LEFTHAND : EquipmentIndex.RIGHTHAND;

					if (type == ItemType.IDT_2HND)
					{
						if (GetItem(otherSide) != null)
							return false;
						else
							return true;
					}
					else if (type == ItemType.IDT_1HND || type == ItemType.IDT_MWPN)
					{
						var otherItem = GetItem(otherSide);
						if (otherItem == null)
							return true;
						if (otherItem.GetItemType() == ItemType.IDT_2HND)
							return false;
						return true;
					}
					else
					{
						return false;
					}
				}
				case EquipmentIndex.EPAULET:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.AMULET:
				{
					return type == ItemType.IDT_NLCE;
				}
				case EquipmentIndex.RING1:
				{
					return type == ItemType.IDT_RING;
				}
				case EquipmentIndex.RING2:
				{
					return type == ItemType.IDT_RING;
				}
				case EquipmentIndex.VEHICLE:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.PET:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.UNKNOWN:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.LEFTEARRING:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.RIGHTEARRING:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.LEFTBRACELET:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.RIGHTBRACELET:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.RING3:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.RING4:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.BELT:
				{
					throw new NotImplementedException(type.ToString());
				}
				case EquipmentIndex.NUM_EQUIPMENT:
				{
					break;
				}
				default:
					// Handle unexpected values
					throw new ArgumentOutOfRangeException(nameof(ei), ei, "Invalid equipment index");
			}
			return false;
		}

		public Item? UnequipItem(UInt16 slot)
		{
			if (slot >= _list.Length)
				return null;
			if (_list[slot] == null)
				return null;

			var item = _list[slot];
			_list[slot] = null;
			return item;
		}

		public bool EquipItem(Character chr, Item item, UInt16 slot)
		{
			if (slot >= _list.Length)
				return false;
			if (_list[slot] != null)
				return false;

			//TODO: check item level, stats etc

			var ok = CheckItemTypeForSlot(chr, item, slot);

			if (!ok)
			{
				return false;
			}

			_list[slot] = item;
			return true;
		}

		public Item? GetItem(EquipmentIndex index)
		{
			if ((int)index < 0 || (int)index >= _list.Length)
				return null;

			var item = _list[(int)index];
			if (item != null && item.Kind == 0)
				return null;
			return item;
		}

		public EquStats GetStats()
		{
			EquStats stats = new EquStats();
			for (int i = 0; i < _list.Length; i++)
			{
				var item = _list[i];
				if (item != null && item.Kind != 0)
				{
					switch ((EquipmentIndex)i)
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
							if (item.GetOpt1() == 8)
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
