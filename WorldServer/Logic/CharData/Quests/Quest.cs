using LibPegasus.Utils;
using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.WorldRuntime;

namespace WorldServer.Logic.CharData.Quests
{
    internal class Quest
	{
		public Quest(UInt16 id)
		{
			if (_questConfig == null)
				throw new Exception("Quest Data not yet loaded");
			Id = id;
			_questInfoMain = _questConfig.MainData[id];
		}

		public Quest(UInt16 id, Boolean started, UInt16 flags, UInt32 actCounter, List<byte>? questProgress) : this(id)
		{
			Started = started;
			Flags = flags;
			ActCounter = actCounter;
			if(questProgress?.Count > 0)
			{
				QuestMobProgress = questProgress;
			}
			else
			{
				QuestMobProgress = null;
			}
			
		}

		public UInt16 Id { get; private set; }
		private static QuestInfo? _questConfig = null;
		private readonly QuestInfoMain _questInfoMain;

		public bool Started { get; private set; }
		public UInt16 Flags { get; private set; }
		public uint ActCounter { get; private set; }
		public List<byte>? QuestMobProgress;

		public void Start()
		{
			Started = true;
			Flags = 0;
			ActCounter = 0;

			if(_questInfoMain.MissionItem?.Length > 0)
			{
				throw new NotImplementedException();
			}
			if (_questInfoMain.MissionDungeon?.Length > 0)
			{
				throw new NotImplementedException();
			}

			if (_questInfoMain.MissionMob?.Length > 0)
			{
				QuestMobProgress = new List<byte>();
				Debug.Assert(_questInfoMain.MissionMob.Length % 2 == 0);
				for (int i = 0; i < _questInfoMain.MissionMob.Length / 2; i++)
				{
					QuestMobProgress.Add(0);
				}
			}
		}

		public QuestReward GetQuestReward()
		{
			if (_questInfoMain.QuestReward != null)
				return _questInfoMain.QuestReward;
			else
				throw new NotImplementedException();
		}

		public int GetStartMapId()
		{
			Debug.Assert(_questInfoMain.OpenNpcs.Length == 2);
			return _questInfoMain.OpenNpcs[0];
		}

		public int GetEndMapId()
		{
			Debug.Assert(_questInfoMain.CloseNpcs.Length == 2);
			return _questInfoMain.CloseNpcs[0];
		}

		public UInt16 GetEndFlags()
		{
			return _questInfoMain.CompletedFlags;
		}

		public int GetStartNpcId()
		{
			Debug.Assert(_questInfoMain.OpenNpcs.Length == 2);
			return _questInfoMain.OpenNpcs[1];
		}

		public int GetEndNpcId()
		{
			Debug.Assert(_questInfoMain.CloseNpcs.Length == 2);
			return _questInfoMain.CloseNpcs[1];
		}

		public bool HasNpcActionSet()
		{
			return _questInfoMain.MissionNPCSet > 0 ? true : false;
		}

		public uint GetExpectedChoice()
		{
			return ActCounter + 1;
		}

		public QuestNpcActionSet GetActionSet(uint choice)
		{
			return _questInfoMain.NpcActionSet[(uint)choice];
		}

		public void AddFlag(uint actIdx, ushort flag)
		{
			if (actIdx != ActCounter + 1)
				throw new Exception("internal quest error");
			ActCounter = actIdx;
			Flags |= flag;
		}

		public static void LoadConfigs(WorldConfig worldConfig)
		{
			if (_questConfig != null) throw new Exception("quest configs already loaded");
			_questConfig = new();

			var cfg = worldConfig.GetConfig("[Quest]");

			foreach (var it in cfg.Values)
			{
				if (it["QuestIdx"] == "<null>")
					continue;
				int QuestIdx = Convert.ToInt32(it["QuestIdx"]);
				int Level = Convert.ToInt32(it["Level"]);
				int maxlv = Convert.ToInt32(it["maxlv"]);
				int MaxRank = Convert.ToInt32(it["MaxRank"]);
				int RankType = Convert.ToInt32(it["RankType"]);
				int[]? BattleStyle = Utility.StringToIntArrayComplex(it["BattleStyle"]);
				int[]? OpenItem = Utility.StringToIntArrayComplex(it["OpenItem"]);
				int[]? OpenSkill = Utility.StringToIntArrayComplex(it["OpenSkill"]);
				int CancelType = Convert.ToInt32(it["CancelType"]);
				int MinReputationClass = Convert.ToInt32(it["MinReputationClass"]);
				int MaxReputationClass = Convert.ToInt32(it["MaxReputationClass"]);
				int PenaltyEXP = Convert.ToInt32(it["PenaltyEXP"]);
				if (!Int32.TryParse(it["MissionNPCSet"], out var MissionNPCSet))
					MissionNPCSet = 0;
				int Reward = Convert.ToInt32(it["Reward"]);
				if (!Int32.TryParse(it["UseDungeon"], out var UseDungeon))
					UseDungeon = 0;
				int[]? MissionItem = Utility.StringToIntArrayComplex(it["MissionItem"]);
				int[]? MissionMob = Utility.StringToIntArrayComplex(it["MissionMob"]);
				int[]? MissionDungeon = Utility.StringToIntArrayComplex(it["MissionDungeon"]);
				int[]? OpenNpcs = Utility.StringToIntArrayComplex(it["OpenNpcs"]);
				int[]? CloseNpcs = Utility.StringToIntArrayComplex(it["CloseNpcs"]);
				int QuestType = Convert.ToInt32(it["QuestType"]);
				int PartyQuest = Convert.ToInt32(it["PartyQuest"]);
				int DeleteType = Convert.ToInt32(it["DeleteType"]);
				int DailyCount = Convert.ToInt32(it["DailyCount"]);
				int Nation_Type = Convert.ToInt32(it["Nation_Type"]);
				int[]? ExclusiveCraft = Utility.StringToIntArrayComplex(it["ExclusiveCraft"]);
				int CommonCraftLevel = Convert.ToInt32(it["CommonCraftLevel"]);
				if (!Int32.TryParse(it["Mission_Player"], out var MissionPlayer))
					MissionPlayer = 0;
				QuestInfoMain data = new(QuestIdx, Level, maxlv, MaxRank, RankType, BattleStyle, OpenItem,
					OpenSkill, CancelType, MinReputationClass, MaxReputationClass, PenaltyEXP, MissionNPCSet,
					Reward, UseDungeon, MissionItem, MissionMob, MissionDungeon, OpenNpcs,
					CloseNpcs, QuestType, PartyQuest, DeleteType, DailyCount, Nation_Type,
					ExclusiveCraft, CommonCraftLevel, MissionPlayer);
				_questConfig.Add(QuestIdx, data);
			}

			cfg = worldConfig.GetConfig("[QuestReward]");

			foreach (var it in cfg.Values)
			{
				int RwdIdx = Convert.ToInt32(it["RwdIdx"]);
				int Order = Convert.ToInt32(it["Order"]);
				int Exp = Convert.ToInt32(it["Exp"]);
				int Alz = Convert.ToInt32(it["Alz"]);
				int MapCode = Convert.ToInt32(it["MapCode"]);
				int WarpCode = Convert.ToInt32(it["WarpCode"]);
				int SkillIdx = 0;
				int RewardItemIdx = 0;
				if (it["SkillIdx"] != "<null>")
					SkillIdx = Convert.ToInt32(it["SkillIdx"]);
				if (it["RewardItemIdx"] != "<null>")
					RewardItemIdx = Convert.ToInt32(it["RewardItemIdx"]);
				int Reputation = Convert.ToInt32(it["Reputation"]);
				int SkillEXP = Convert.ToInt32(it["SkillEXP"]);
				int AXP = Convert.ToInt32(it["AXP"]);
				int CraftEXP = Convert.ToInt32(it["CraftEXP"]);
				int PetEXP = Convert.ToInt32(it["PetEXP"]);
				int GuildEXP = Convert.ToInt32(it["GuildEXP"]);

				if(_questConfig.MainData.TryGetValue(RwdIdx, out var data))
				{
					data.SetQuestReward(new QuestReward(RwdIdx, Order, Exp, Alz, MapCode, WarpCode, SkillIdx, RewardItemIdx, Reputation, SkillEXP, AXP, CraftEXP, PetEXP, GuildEXP));
				}
				else
				{
					throw new Exception("Something's not quite right");
				}
			}

			cfg = worldConfig.GetConfig("[NpcActionSet]");

			foreach (var it in cfg.Values)
			{
				int SetIdx = Convert.ToInt32(it["SetIdx"]);
				int Order = Convert.ToInt32(it["Order"]);
				int[]? ActNpc = Utility.StringToIntArray(it["ActNpc"]);
				int ActIdx = Convert.ToInt32(it["ActIdx"]);
				if (!Int32.TryParse(it["ItemKind/Code"], out var ItemKindCode))
					ItemKindCode = 0;
				if (!Int32.TryParse(it["ItemOpt"], out var ItemOpt))
					ItemOpt = 0;
				int Action = Convert.ToInt32(it["Action"]);
				_questConfig.AddNpcActionSet(SetIdx, new(SetIdx, Order, ActNpc, ActIdx, ItemKindCode, ItemOpt, (NpcActionType)Action));
			}
		}
	}

	internal class QuestAction
	{
		public uint ChoiceId;
		public ushort Unknown;
	}
}
