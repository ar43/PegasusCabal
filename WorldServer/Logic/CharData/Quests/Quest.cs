using LibPegasus.Utils;
using System.Diagnostics;
using WorldServer.Enums;
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
			QuestInfoMain = _questConfig.MainData[id];
		}

		public Quest(UInt16 id, Boolean started, UInt16 flags, UInt32 actCounter, List<byte>? questProgress) : this(id)
		{
			Started = started;
			Flags = flags;
			ActCounter = actCounter;
			MobProgress = null;
			ItemProgress = null;
			if (questProgress?.Count > 0)
			{
				int mobLen = QuestInfoMain.MissionMob == null ? 0 : QuestInfoMain.MissionMob.Length / 2;
				int itemLen = QuestInfoMain.MissionItem == null ? 0 : QuestInfoMain.MissionItem.Length;
				int dungeonLen = QuestInfoMain.MissionDungeon == null ? 0 : QuestInfoMain.MissionDungeon.Length;
				if (mobLen > 0)
				{
					MobProgress = questProgress.Slice(0, mobLen);
				}
				if (itemLen > 0)
				{
					ItemProgress = questProgress.Slice(mobLen, itemLen);
				}
				if (dungeonLen > 0)
				{
					DungeonProgress = questProgress.Slice(itemLen + mobLen, dungeonLen);
				}
			}

		}

		public UInt16 Id { get; private set; }
		private static QuestInfo? _questConfig = null;
		public readonly QuestInfoMain QuestInfoMain;

		public bool Started { get; private set; }
		public UInt16 Flags { get; private set; }
		public uint ActCounter { get; private set; }
		public List<byte>? MobProgress;
		public List<byte>? ItemProgress;
		public List<byte>? DungeonProgress;
		public ulong StoredInstanceId { get; private set; }

		public void StoreInstanceId(ulong id)
		{
			if (StoredInstanceId != 0)
				throw new Exception("Already set instance id");
			StoredInstanceId = id;
		}

		public void Start()
		{
			int debugCountProgressTypes = 0;

			Started = true;
			Flags = 0;
			ActCounter = 0;

			if (QuestInfoMain.MissionItem?.Length > 0)
			{
				debugCountProgressTypes++;
				ItemProgress = new List<byte>();

				foreach (var tuple in QuestInfoMain.MissionItem)
				{
					ItemProgress.Add(0);
				}

			}
			if (QuestInfoMain.MissionDungeon?.Length > 0)
			{
				Debug.Assert(QuestInfoMain.MissionDungeon.Length == 1);
				debugCountProgressTypes++;
				DungeonProgress = new List<byte>();

				foreach (var dungId in QuestInfoMain.MissionDungeon)
				{
					DungeonProgress.Add(0);
				}
			}

			if (QuestInfoMain.MissionMob?.Length > 0)
			{
				debugCountProgressTypes++;
				MobProgress = new List<byte>();
				Debug.Assert(QuestInfoMain.MissionMob.Length % 2 == 0);
				for (int i = 0; i < QuestInfoMain.MissionMob.Length / 2; i++)
				{
					MobProgress.Add(0);
				}
			}

			if (debugCountProgressTypes >= 2)
			{
				throw new NotImplementedException("unimplemented combo quest - " + Id.ToString());
			}
		}

		public bool OnMobDeath(int mobId)
		{
			if (Started == false || MobProgress == null || Flags != GetEndFlags())
				return false;

			for (int i = 0; i < QuestInfoMain.MissionMob.Length; i++)
			{
				if (i % 2 == 0)
				{
					if (QuestInfoMain.MissionMob[i] == mobId)
					{
						var currentProgress = MobProgress[i / 2];
						if (currentProgress < QuestInfoMain.MissionMob[i + 1])
						{
							MobProgress[i / 2]++;
							return true;
						}
						else
						{
							return false;
						}
					}
				}
			}

			return false;
		}



		public QuestReward GetQuestReward()
		{
			if (QuestInfoMain.QuestReward != null)
				return QuestInfoMain.QuestReward;
			else
				throw new NotImplementedException();
		}

		public int GetStartMapId()
		{
			Debug.Assert(QuestInfoMain.OpenNpcs.Length == 2);
			return QuestInfoMain.OpenNpcs[0];
		}

		public int GetEndMapId()
		{
			Debug.Assert(QuestInfoMain.CloseNpcs.Length == 2);
			return QuestInfoMain.CloseNpcs[0];
		}

		public UInt16 GetEndFlags()
		{
			return QuestInfoMain.CompletedFlags;
		}

		public int GetStartNpcId()
		{
			Debug.Assert(QuestInfoMain.OpenNpcs.Length == 2);
			return QuestInfoMain.OpenNpcs[1];
		}

		public int GetEndNpcId()
		{
			Debug.Assert(QuestInfoMain.CloseNpcs.Length == 2);
			return QuestInfoMain.CloseNpcs[1];
		}

		public bool HasNpcActionSet()
		{
			return QuestInfoMain.MissionNPCSet > 0 ? true : false;
		}

		public uint GetExpectedChoice()
		{
			return ActCounter + 1;
		}

		public QuestNpcActionSet GetActionSet(uint choice)
		{
			return QuestInfoMain.NpcActionSet[(uint)choice];
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
					Reward, UseDungeon, Utility.IntArrayToTupleArray3(MissionItem), MissionMob, MissionDungeon, OpenNpcs,
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

				if (_questConfig.MainData.TryGetValue(RwdIdx, out var data))
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
		public ushort Param; //seems to be inventory Slot, maybe can also be sth else?
	}
}
