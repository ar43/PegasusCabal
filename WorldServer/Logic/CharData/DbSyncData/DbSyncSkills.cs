using Shared.Protos;

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
