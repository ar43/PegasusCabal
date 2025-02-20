﻿using Google.Protobuf;
using Microsoft.VisualBasic;
using Shared.Protos;
using System.Collections;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.CharData.Quests
{
	internal class QuestManager
	{
		public DBSyncPriority SyncPending { get; private set; }
		public Dictionary<int, Quest> ActiveQuests { get; private set; }
		private BitArray _startedQuests;
		public BitArray CompletedQuests { get; private set; }
		public BitArray CompletedDungeons { get; private set; }

		public QuestManager()
		{
			_startedQuests = new(1023 * 8);
			CompletedQuests = new(1023 * 8);
			CompletedDungeons = new(128 * 8);
			ActiveQuests = new();
		}

		public DbSyncQuest GetDB()
		{
			return new DbSyncQuest(GetActiveProtobuf(), GetCompletedProtobuf());
		}

		private CompletedQuestsData GetCompletedProtobuf()
		{
			CompletedQuestsData data = new CompletedQuestsData();
			byte[] bytes = new byte[(CompletedQuests.Length - 1) / 8 + 1];
			byte[] bytes2 = new byte[(CompletedDungeons.Length - 1) / 8 + 1];

			CompletedQuests.CopyTo(bytes, 0);
			CompletedDungeons.CopyTo(bytes2, 0);


			data.CompletedQuests = ByteString.CopyFrom(bytes);
			data.CompletedDungeons = ByteString.CopyFrom(bytes2);

			return data;
		}

		public void Sync(DBSyncPriority prio)
		{
			if (SyncPending < prio)
				SyncPending = prio;
			if (prio == DBSyncPriority.NONE)
				SyncPending = DBSyncPriority.NONE;
		}

		public void OnMobDeath(Client client, ushort mobId, int skillId)
		{
			foreach (var quest in ActiveQuests.Values)
			{
				if (quest.OnMobDeath(mobId))
				{
					NFY_QuestMobKilled success = new(mobId, skillId);
					client.PacketManager.Send(success);
				}
			}
		}

		private ActiveQuestData GetActiveProtobuf()
		{
			ActiveQuestData data = new ActiveQuestData();

			foreach (var it in ActiveQuests)
			{
				ByteString progress = ByteString.Empty;
				List<byte> finalProgress = new List<byte>();
				if (it.Value.MobProgress?.Count > 0)
					finalProgress.AddRange(it.Value.MobProgress);
				if (it.Value.ItemProgress?.Count > 0)
					finalProgress.AddRange(it.Value.ItemProgress);
				if (it.Value.DungeonProgress?.Count > 0)
					finalProgress.AddRange(it.Value.DungeonProgress);

				if (finalProgress.Count > 0)
					progress = ByteString.CopyFrom(finalProgress.ToArray());

				data.ActiveQuestData_.Add((uint)it.Key, new ActiveQuestData.Types.ActiveQuestDataItem { Flag = it.Value.Flags, Id = it.Value.Id, IsExpanded = true, IsTracked = true, Progress = progress, ActCounter = it.Value.ActCounter, Slot = (uint)it.Key, Started = it.Value.Started });
			}

			return data;
		}

		public void StartQuest(int questId, int slot, Location posData, Dictionary<Int32, WorldRuntime.MapDataRuntime.NpcData> npcData, Character character)
		{
			if (ActiveQuests.ContainsKey(slot) || _startedQuests[questId] == true)
				throw new Exception("Either the slot is full or quest is already started");

			Debug.Assert(character.Style.BattleStyleNum > 0);
			var quest = new Quest((UInt16)questId);
			var npcPosX = npcData[quest.GetStartNpcId()].PosX;
			var npcPosY = npcData[quest.GetStartNpcId()].PosY;
			var bStyle = 1 << (character.Style.BattleStyleNum - 1);

			if (posData.Instance?.MapId != (MapId)quest.GetStartMapId())
				throw new Exception("char not in correct instance");
			if (!posData.Movement.VerifyDistanceToNpc(npcPosX, npcPosY))
				throw new Exception("char too far away from npc");
			if(quest.QuestInfoMain.Level > character.Stats.Level)
				throw new Exception("level too low");
			if ((quest.QuestInfoMain.BattleStyle[0] & bStyle) != bStyle)
				throw new Exception("incorrect battle style");
			if (quest.QuestInfoMain.BattleStyle[1] > character.Style.MasteryLevel)
				throw new Exception("incorrect battle style mastery level");

			quest.Start();
			_startedQuests[questId] = true;
			ActiveQuests[slot] = quest;
		}

		internal UInt32 EndQuest(UInt16 questId, UInt16 slot, Location posData, Dictionary<Int32, NpcData> npcData, Client client, UInt16 choice, UInt16 invSlot)
		{
			ActiveQuests.TryGetValue(slot, out var quest);
			var inv = client.Character.Inventory;

			if (quest == null)
				throw new Exception("quest does not exist in the slot");
			if (quest.Id != questId)
				throw new Exception("incorrect quest");

			if (quest.Flags != quest.GetEndFlags())
				throw new Exception("cant complete the quest yet");

			var npcPosX = npcData[quest.GetEndNpcId()].PosX;
			var npcPosY = npcData[quest.GetEndNpcId()].PosY;
			bool isRangeCheck = npcData[quest.GetEndNpcId()].IsRangeCheck;

			if (posData.Instance?.MapId != (MapId)quest.GetEndMapId())
				throw new Exception("char not in correct instance");
			if (isRangeCheck && !posData.Movement.VerifyDistanceToNpc(npcPosX, npcPosY))
				throw new Exception("char too far away from npc");

			if (quest.ItemProgress != null)
			{
				int i = 0;
				foreach (var prog in quest.ItemProgress)
				{
					if (prog != quest.QuestInfoMain.MissionItem[i].Item3)
					{
						throw new Exception("quest item mission not completed");
					}
					bool success = inv.RemoveAllQuestItemsByKind((UInt32)quest.QuestInfoMain.MissionItem[i].Item1) >= quest.QuestInfoMain.MissionItem[i].Item3;
					if (!success)
					{
						throw new Exception("quest item mission not completed (SHOULD NOT HAPPEN)");
					}
					i++;
				}
			}

			if (quest.MobProgress != null)
			{
				int i = 0;
				foreach (var prog in quest.MobProgress)
				{
					if (prog != quest.QuestInfoMain.MissionMob[i * 2 + 1])
					{
						throw new Exception("quest mob mission not completed");
					}
					i++;
				}
			}

			if (quest.DungeonProgress != null)
			{
				foreach (var prog in quest.DungeonProgress)
				{
					if (prog < 1)
					{
						throw new Exception("quest mission dungeon not completed");
					}
				}
			}

			var questReward = quest.GetQuestReward();

			Item? itemReward = null;

			if (questReward.RewardItemIdx > 0)
			{
				itemReward = Item.GenerateReward((UInt32)questReward.RewardItemIdx, client.Character.Style.BattleStyleNum, choice);
			}

			if (itemReward != null)
			{
				if (!inv.AddItem(invSlot, itemReward))
					throw new Exception("failed to add reward item");
			}

			client.Character.Stats.AddExp((UInt32)questReward.Exp);
			inv.GiveAlz((UInt64)questReward.Alz);

			//TODO:
			/*
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
			*/

			//send packet 760 NFY_SkillStatus

			var exp = (UInt32)questReward.Exp;

			ActiveQuests.Remove(slot);
			CompletedQuests[questId] = true;

			if(quest.Type() == 4) //BSLVUP
			{
				var newMasteryLevel = quest.QuestInfoMain.BattleStyle[1] + 1;
				client.Character.Style.SetMasteryLevel((byte)newMasteryLevel);

				var nfyMslvup = new NFY_SMastUpEvnt(client.Character.Style.MasteryLevel);
				client.PacketManager.Send(nfyMslvup);
			}

			return exp;
		}

		internal Quest OnNpcAction(UInt16 questId, Location posData, Dictionary<Int32, NpcData> npcData, List<QuestAction> questActions, Inventory inv)
		{
			Quest? quest = null;
			foreach (var it in ActiveQuests.Values)
			{
				if (it.Id == questId)
					quest = it;
			}
			if (quest == null)
				throw new Exception("Quest not found");

			if (quest.Started == false)
				throw new Exception("Quest not started");

			if (!quest.HasNpcActionSet())
				throw new Exception("Quest is not progressable that way");

			foreach (var it in questActions)
			{
				var actionSet = quest.GetActionSet(it.ChoiceId);

				if (actionSet == null)
					throw new Exception("bad action");

				uint actIdx = (uint)actionSet.ActIdx;
				Debug.Assert(actIdx == it.ChoiceId);
				var expectedIdx = quest.GetExpectedChoice();

				if (actIdx != expectedIdx)
					throw new Exception("action out of order");

				if (posData.Instance?.MapId != (MapId)actionSet.ActNpc[0])
					throw new Exception("char not in correct instance");

				var npcPosX = npcData[actionSet.ActNpc[1]].PosX;
				var npcPosY = npcData[actionSet.ActNpc[1]].PosY;
				if (npcData[actionSet.ActNpc[1]].IsRangeCheck && !posData.Movement.VerifyDistanceToNpc(npcPosX, npcPosY))
					throw new Exception("char too far away from npc");

				if (actionSet.Action == NpcActionType.QACT_GIVE)
				{
					Serilog.Log.Debug($"it.Unknown: {it.Param}");
					inv.AddItem(it.Param, new Item((UInt32)actionSet.ItemKindCode, (UInt32)actionSet.ItemOpt, 0, 0));
				}
				else if (actionSet.Action == NpcActionType.QACT_TAKE)
				{
					Serilog.Log.Debug($"it.Unknown: {it.Param}");
					var item = inv.PeekItem(it.Param);
					if (item == null)
						throw new Exception("item slot empty");

					if (item.Kind == (UInt32)actionSet.ItemKindCode && item.Option == (UInt32)actionSet.ItemOpt)
					{
						_ = inv.RemoveItem(it.Param);
					}
					else
					{
						throw new Exception("invalid item");
					}
				}
				else if(actionSet.Action == NpcActionType.QACT_CTRG)
				{
					if(posData.Instance == null || (posData.Instance.Id != quest.StoredInstanceId && quest.StoredInstanceId > 0) || posData.Instance.MissionDungeonManager == null ||
						(quest.QuestInfoMain.ExtraDungeonInfo.Count > 0 && !quest.QuestInfoMain.ExtraDungeonInfo.Contains(posData.Instance.MissionDungeonManager.GetDungeonId())))
					{
						throw new NotImplementedException("wrong instance");
					}

					posData.Instance.MissionDungeonManager.NpcTrigger(it.Param, actionSet.ActNpc[1]);
				}
				else if (actionSet.Action != NpcActionType.QACT_TALK)
				{
					throw new NotImplementedException("Unimplemented NpcActionType");
				}

				quest.AddFlag(actIdx, (ushort)(1 << actionSet.Order));
			}


			return quest;
		}

		internal void SetCompletedQuests(CompletedQuestsData? questsCompletedProtobuf)
		{
			if (questsCompletedProtobuf != null)
			{
				CompletedQuests = new BitArray(questsCompletedProtobuf.CompletedQuests.ToByteArray());
				if(questsCompletedProtobuf.CompletedDungeons.ToByteArray().Length > 0)
					CompletedDungeons = new BitArray(questsCompletedProtobuf.CompletedDungeons.ToByteArray());
			}
		}

		internal void SetActiveQuests(ActiveQuestData? questsActiveProtobuf)
		{
			if (questsActiveProtobuf != null)
			{
				ActiveQuests = new();

				foreach (var it in questsActiveProtobuf.ActiveQuestData_)
				{
					Quest quest = new((ushort)it.Value.Id, it.Value.Started, (ushort)it.Value.Flag, it.Value.ActCounter, new List<Byte>(it.Value.Progress));

					ActiveQuests[(int)it.Key] = quest;
				}
			}
		}

		public byte[] Serialize()
		{
			var bytes = new List<byte>();
			foreach (var quest in ActiveQuests)
			{
				if (quest.Value != null)
				{
					bytes.AddRange(BitConverter.GetBytes(quest.Value.Id));
					bytes.AddRange(BitConverter.GetBytes(quest.Value.Flags));
					bytes.Add(1); //TODO - SAVE ISTRACKED AND ISEXPANDED
					bytes.Add(1); //TODO - SAVE ISTRACKED AND ISEXPANDED
					bytes.Add((byte)quest.Key); //TODO - SAVE ISTRACKED AND ISEXPANDED
					if (quest.Value.MobProgress?.Count > 0)
						bytes.AddRange(quest.Value.MobProgress);
					if (quest.Value.ItemProgress?.Count > 0)
						bytes.AddRange(quest.Value.ItemProgress);
					//if (quest.Value.DungeonProgress?.Count > 0)
					//	bytes.AddRange(quest.Value.DungeonProgress);
					//TODO - add progress for other quests
				}
			}
			return bytes.ToArray();
		}

		internal void SetStartedQuests()
		{
			_startedQuests = new(CompletedQuests);

			foreach (var it in ActiveQuests.Values)
			{
				_startedQuests[it.Id] = true;
			}
		}

		//debug only
		internal void Reset(bool activeOnly = false)
		{
#if DEBUG
			if(!activeOnly)
			{
				_startedQuests = new(1023 * 8);
				CompletedQuests = new(1023 * 8);
				CompletedDungeons = new(128 * 8);
			}
			ActiveQuests = new();
#endif
		}

		internal (int, int, int) NeedItem(Item item)
		{
			Debug.Assert(item.IsQuestItem());

			var real_opt = item.GetQuestItemOpt();
			var real_quant = item.GetQuestItemCount();

			foreach (var quest in ActiveQuests)
			{
				if (quest.Value.ItemProgress != null)
				{
					int i = 0;
					foreach (var lootProgress in quest.Value.ItemProgress)
					{
						var neededKind = quest.Value.QuestInfoMain.MissionItem[i].Item1;
						var neededOpt = quest.Value.QuestInfoMain.MissionItem[i].Item2;
						var neededCount = quest.Value.QuestInfoMain.MissionItem[i].Item3;

						var remaining = neededCount - lootProgress;

						if (neededKind == item.Kind && neededOpt == real_opt && remaining > 0 && remaining - real_quant >= 0)
						{
							return (quest.Key, i, (int)real_quant);
						}

						i++;
					}
				}
			}

			return (0, 0, 0);
		}

		internal void OnQuestItemLoot(int slot, int lootIndex, int lootQuant)
		{
			var quest = ActiveQuests[slot];
			quest.ItemProgress[lootIndex] += (Byte)lootQuant;
		}

		internal void OnDungeonCompleted(int dungeonId)
		{
			foreach(var quest in ActiveQuests.Values)
			{
				if(quest.QuestInfoMain.MissionDungeon != null && quest.QuestInfoMain.MissionDungeon[0] == dungeonId)
				{
					quest.DungeonProgress[0] = 1;
					if (dungeonId < 4096)
						throw new Exception("dungeonId < 4096");
					dungeonId -= 4096;
					CompletedDungeons[dungeonId] = true;
				}
			}
		}
	}
}
