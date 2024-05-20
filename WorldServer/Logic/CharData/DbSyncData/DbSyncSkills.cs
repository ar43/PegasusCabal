using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncSkills
	{
		public DbSyncSkills(SkillData skillData)
		{
			SkillData = skillData;
		}

		public SkillData SkillData { get; private set; }

	}
}
