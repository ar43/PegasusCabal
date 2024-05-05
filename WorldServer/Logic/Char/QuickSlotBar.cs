using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Char
{
	internal class QuickSlotBar
	{
		public Dictionary<UInt16, SkillLink> Links;

		public QuickSlotBar()
		{
			Links = new Dictionary<UInt16, SkillLink>();
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
