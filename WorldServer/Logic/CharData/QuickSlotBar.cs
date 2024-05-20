using Google.Protobuf;
using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			if(protobuf != null)
			{
				foreach (var link in protobuf.QuickSlotData_)
				{
					_links.Add((UInt16)link.Key, new SkillLink((UInt16)link.Value.Id));
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

		public QuickSlotData GetProtobuf()
		{
			QuickSlotData data = new QuickSlotData();
			foreach(var linkKeyPar in _links)
			{
				var slot = linkKeyPar.Key;
				var link = linkKeyPar.Value;
				data.QuickSlotData_.Add(slot, new QuickSlotData.Types.QuickSlotDataItem { Id = link.Id });
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
					bytes.AddRange(BitConverter.GetBytes(link.Value.Id));
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
