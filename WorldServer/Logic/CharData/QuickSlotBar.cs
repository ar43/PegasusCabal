using Shared.Protos;
using WorldServer.Enums;

namespace WorldServer.Logic.CharData
{
	internal class QuickSlotBar
	{
		private Dictionary<UInt16, SkillLink> _links;
		public DBSyncPriority SyncPending { get; private set; }

		public QuickSlotBar(QuickSlotData? protobuf)
		{
			_links = new Dictionary<UInt16, SkillLink>();
			SyncPending = DBSyncPriority.NONE;

			if (protobuf != null)
			{
				foreach (var link in protobuf.QuickSlotData_)
				{
					_links.Add((UInt16)link.Key, new SkillLink((UInt16)link.Value.Id));
				}
			}
		}

		public void Set(Int16 quickSlot, Int16 skillSlotId)
		{
			//TODO: verify
			if(skillSlotId == -1)
			{
				if (!_links.Remove((ushort)quickSlot))
					throw new Exception("quick slot was already empty");
			}
			else
			{
				_links[(ushort)quickSlot] = new SkillLink((ushort)skillSlotId);
			}
		}

		public void Swap(Int16 quickSlot1, Int16 quickSlot2)
		{
			//TODO: verify
			var temp1 = _links[(ushort)quickSlot1];
			var temp2 = _links[(ushort)quickSlot1];
			if (temp1 == null || temp2 == null)
				throw new Exception("quickSlot1 is empty");
			_links[(ushort)quickSlot1] = _links[(ushort)quickSlot2];
			_links[(ushort)quickSlot2] = temp1;
		}

		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public QuickSlotData GetProtobuf()
		{
			QuickSlotData data = new QuickSlotData();
			foreach (var linkKeyPar in _links)
			{
				var slot = linkKeyPar.Key;
				var link = linkKeyPar.Value;
				data.QuickSlotData_.Add(slot, new QuickSlotData.Types.QuickSlotDataItem { Id = link.SkillSlotId });
			}
			return data;
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			foreach (var link in _links)
			{
				if (link.Value != null)
				{
					bytes.AddRange(BitConverter.GetBytes(link.Value.SkillSlotId));
					bytes.AddRange(BitConverter.GetBytes(link.Key));
				}
			}
			return bytes.ToArray();
		}

		public int Count()
		{
			return _links.Count;
		}
	}
}
