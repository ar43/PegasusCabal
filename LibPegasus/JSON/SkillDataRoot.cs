using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.JSON
{
	public class SkillDataRoot
	{
		public SkillDataRoot(Dictionary<UInt16, SkillDataEntry> skillData)
		{
			SkillData = skillData;
		}

		public Dictionary<UInt16, SkillDataEntry> SkillData {  get; set; }

	}

	public class SkillDataEntry
	{
		public SkillDataEntry(UInt16 id, Byte level)
		{
			Id = id;
			Level = level;
		}

		public UInt16 Id { get; set; }
		public byte Level { get; set;}
	}
}
