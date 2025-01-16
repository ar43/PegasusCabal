using Shared.Protos;
using WorldServer.Enums;

namespace WorldServer.Logic.CharData.Skills
{
	internal class LearnedSkills
	{
		private Dictionary<UInt16, Skill> _learnedSkills; //make private
		public DBSyncPriority SyncPending { get; private set; }

		public LearnedSkills(SkillData? protobuf)
		{
			_learnedSkills = new Dictionary<UInt16, Skill>();
			SyncPending = DBSyncPriority.NONE;

			if (protobuf != null)
			{
				foreach (var skill in protobuf.SkillData_)
				{
					_learnedSkills.Add((UInt16)skill.Key, new Skill((UInt16)skill.Value.Id, (Byte)skill.Value.Level));
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

		public bool HasSkill(UInt16 skillSlot, UInt16 skillId)
		{
			if (_learnedSkills.ContainsKey(skillSlot))
				return _learnedSkills[skillSlot].Id == skillId;
			else
				return false;
		}

		public Skill Get(int slot)
		{
			return _learnedSkills[(UInt16)slot];
		}

		//temp
		public void DebugAddSkill(UInt16 skillSlot, Skill skill)
		{
			_learnedSkills[skillSlot] = skill;
		}

		public SkillData GetProtobuf()
		{
			SkillData skillData = new SkillData();
			foreach (var skillKeyPair in _learnedSkills)
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
			foreach (var skill in _learnedSkills)
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

		public int Count()
		{
			return _learnedSkills.Count;
		}
	}
}
