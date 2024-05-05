using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Char
{
	internal class Skills
	{
		public Dictionary<UInt16, Skill> LearnedSkills;

		public Skills()
		{
			LearnedSkills = new Dictionary<UInt16, Skill>();
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			foreach (var skill in LearnedSkills)
			{
				if (skill.Value != null)
				{
					bytes.AddRange(BitConverter.GetBytes(skill.Value.Id));
					bytes.Add(skill.Value.Level);
					bytes.AddRange(BitConverter.GetBytes(skill.Key));
				}
			}
			return bytes.ToArray();
		}
	}
}
