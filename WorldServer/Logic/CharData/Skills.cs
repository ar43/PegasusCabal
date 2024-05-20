using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Logic.CharData
{
	internal class Skills
	{
		public Dictionary<UInt16, Skill> LearnedSkills; //make private
		public DBSyncPriority SyncPending { get; private set; }

		public Skills()
		{
			LearnedSkills = new Dictionary<UInt16, Skill>();
			SyncPending = DBSyncPriority.NONE;
		}

		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
		}

		public SkillData GetProtobuf()
		{
			SkillData skillData = new SkillData();
			foreach(var skillKeyPair in LearnedSkills)
			{
				var skill = skillKeyPair.Value;
				var slot = skillKeyPair.Key;

				skillData.SkillData_.Add(slot, new SkillData.Types.SkillDataItem { Id = skill.Id, Level = skill.Level });
			}
			return skillData;
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
