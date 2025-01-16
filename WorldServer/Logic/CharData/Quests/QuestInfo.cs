using WorldServer.Enums;

namespace WorldServer.Logic.CharData.Quests
{
	internal class QuestInfo
	{
		public QuestInfo()
		{
			MainData = new();
		}

		public Dictionary<int, QuestInfoMain> MainData { get; private set; }

		public void Add(int id, QuestInfoMain mainInfo)
		{
			MainData.Add(id, mainInfo);
		}

		public void AddNpcActionSet(int questId, QuestNpcActionSet set)
		{
			if (MainData[questId].NpcActionSet.ContainsKey((uint)set.ActIdx))
				throw new Exception("Re-think");

			MainData[questId].NpcActionSet[(uint)set.ActIdx] = set;
			MainData[questId].CompletedFlags |= (UInt16)(1 << set.Order);
		}

	}

	internal class QuestNpcActionSet
	{
		public QuestNpcActionSet(Int32 setIdx, Int32 order, Int32[]? actNpc, Int32 actIdx, Int32 itemKindCode, Int32 itemOpt, NpcActionType action)
		{
			SetIdx = setIdx;
			Order = order;
			ActNpc = actNpc;
			ActIdx = actIdx;
			ItemKindCode = itemKindCode;
			ItemOpt = itemOpt;
			Action = action;
		}

		public int SetIdx { get; private set; }
		public int Order { get; private set; }
		public int[]? ActNpc { get; private set; }
		public int ActIdx { get; private set; }
		public int ItemKindCode { get; private set; }
		public int ItemOpt { get; private set; }
		public NpcActionType Action { get; private set; }
	}

	internal class QuestReward
	{
		public QuestReward(Int32 rwdIdx, Int32 order, Int32 exp, Int32 alz, Int32 mapCode, Int32 warpCode, Int32 skillIdx, Int32 rewardItemIdx, Int32 reputation, Int32 skillEXP, Int32 aXP, Int32 craftEXP, Int32 petEXP, Int32 guildEXP)
		{
			RwdIdx = rwdIdx;
			Order = order;
			Exp = exp;
			Alz = alz;
			MapCode = mapCode;
			WarpCode = warpCode;
			SkillIdx = skillIdx;
			RewardItemIdx = rewardItemIdx;
			Reputation = reputation;
			SkillEXP = skillEXP;
			AXP = aXP;
			CraftEXP = craftEXP;
			PetEXP = petEXP;
			GuildEXP = guildEXP;
		}

		public int RwdIdx { get; private set; }
		public int Order { get; private set; }
		public int Exp { get; private set; }
		public int Alz { get; private set; }
		public int MapCode { get; private set; }
		public int WarpCode { get; private set; }
		public int SkillIdx { get; private set; }
		public int RewardItemIdx { get; private set; }
		public int Reputation { get; private set; }
		public int SkillEXP { get; private set; }
		public int AXP { get; private set; }
		public int CraftEXP { get; private set; }
		public int PetEXP { get; private set; }
		public int GuildEXP { get; private set; }
	}

	internal class QuestInfoMain
	{
		public QuestInfoMain(Int32 questIdx, Int32 level, Int32 maxlv, Int32 maxRank, Int32 rankType, Int32[]? battleStyle, Int32[]? openItem, Int32[]? openSkill, Int32 cancelType, Int32 minReputationClass, Int32 maxReputationClass, Int32 penaltyEXP, Int32 missionNPCSet, Int32 reward, Int32 useDungeon, (int, int, int)[]? missionItem, Int32[]? missionMob, Int32[]? missionDungeon, Int32[]? openNpcs, Int32[]? closeNpcs, Int32 questType, Int32 partyQuest, Int32 deleteType, Int32 dailyCount, Int32 nation_Type, Int32[]? exclusiveCraft, Int32 commonCraftLevel, Int32 mission_Player)
		{
			QuestIdx = questIdx;
			Level = level;
			this.maxlv = maxlv;
			MaxRank = maxRank;
			RankType = rankType;
			BattleStyle = battleStyle;
			OpenItem = openItem;
			OpenSkill = openSkill;
			CancelType = cancelType;
			MinReputationClass = minReputationClass;
			MaxReputationClass = maxReputationClass;
			PenaltyEXP = penaltyEXP;
			MissionNPCSet = missionNPCSet;
			Reward = reward;
			UseDungeon = useDungeon;
			MissionItem = missionItem;
			MissionMob = missionMob;
			MissionDungeon = missionDungeon;
			OpenNpcs = openNpcs;
			CloseNpcs = closeNpcs;
			QuestType = questType;
			PartyQuest = partyQuest;
			DeleteType = deleteType;
			DailyCount = dailyCount;
			Nation_Type = nation_Type;
			ExclusiveCraft = exclusiveCraft;
			CommonCraftLevel = commonCraftLevel;
			Mission_Player = mission_Player;
			NpcActionSet = new();
			QuestReward = null;
			CompletedFlags = 0;
		}

		public int QuestIdx { get; private set; }
		public int Level { get; private set; }
		public int maxlv { get; private set; }
		public int MaxRank { get; private set; }
		public int RankType { get; private set; }
		public int[]? BattleStyle { get; private set; }
		public int[]? OpenItem { get; private set; }
		public int[]? OpenSkill { get; private set; }
		public int CancelType { get; private set; }
		public int MinReputationClass { get; private set; }
		public int MaxReputationClass { get; private set; }
		public int PenaltyEXP { get; private set; }
		public int MissionNPCSet { get; private set; }
		public int Reward { get; private set; }
		public int UseDungeon { get; private set; }
		public (int, int, int)[]? MissionItem { get; private set; }
		public int[]? MissionMob { get; private set; }
		public int[]? MissionDungeon { get; private set; }
		public int[]? OpenNpcs { get; private set; }
		public int[]? CloseNpcs { get; private set; }
		public int QuestType { get; private set; }
		public int PartyQuest { get; private set; }
		public int DeleteType { get; private set; }
		public int DailyCount { get; private set; }
		public int Nation_Type { get; private set; }
		public int[]? ExclusiveCraft { get; private set; }
		public int CommonCraftLevel { get; private set; }
		public int Mission_Player { get; private set; }
		public QuestReward? QuestReward { get; private set; }
		public Dictionary<uint, QuestNpcActionSet> NpcActionSet { get; private set; }
		public UInt16 CompletedFlags { get; set; }

		public void SetQuestReward(QuestReward rwd)
		{
			if (QuestReward == null && Reward == rwd.RwdIdx)
				QuestReward = rwd;
			else
				throw new Exception("Something's not right");
		}
	}
}
