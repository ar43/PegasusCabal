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
		public Dictionary<UInt16, SkillLink> Links;
		public DBSyncPriority SyncPending { get; private set; }

		public QuickSlotBar()
		{
			Links = new Dictionary<UInt16, SkillLink>();
			SyncPending = DBSyncPriority.NONE;
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
			foreach(var linkKeyPar in Links)
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
			foreach (var link in Links)
			{
				if (link.Value != null)
				{
					bytes.AddRange(BitConverter.GetBytes(link.Value.Id));
					bytes.AddRange(BitConverter.GetBytes(link.Key));
				}
			}
			return bytes.ToArray();
		}
	}
}
