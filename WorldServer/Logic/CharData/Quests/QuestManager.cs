using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;

namespace WorldServer.Logic.CharData.Quests
{
    internal class QuestManager
    {
		private Dictionary<int, Quest> _activeQuests = new();
		private BitArray _startedQuests;
		private BitArray _completedQuests;

		public QuestManager()
		{
			_startedQuests = new(1023 * 8);
			_completedQuests = new(1023 * 8);
		}

		public void StartQuest(int questId, int slot, Location posData, Dictionary<Int32, WorldRuntime.MapDataRuntime.NpcData> npcData)
		{
			if (_activeQuests.ContainsKey(slot) || _startedQuests[questId] == true)
				throw new Exception("Either the slot is full or quest is already started");

			var quest = new Quest((UInt16)questId);
			var npcPosX = npcData[quest.GetStartNpcId()].PosX;
			var npcPosY = npcData[quest.GetStartNpcId()].PosY;

			if (posData.Instance?.MapId != (MapId)quest.GetStartMapId())
				throw new Exception("char not in correct instance");
			if (!posData.Movement.VerifyDistanceToNpc(npcPosX, npcPosY))
				throw new Exception("char too far away from npc");

			_startedQuests[questId] = true;
			_activeQuests[slot] = quest;
			quest.Start();
		}

		internal UInt32 EndQuest(UInt16 questId, UInt16 slot, Location posData, Dictionary<Int32, NpcData> npcData, Client client, UInt16 choice, UInt16 invSlot)
		{
			_activeQuests.TryGetValue(slot, out var quest);
			var inv = client.Character.Inventory;

			if (quest == null)
				throw new Exception("quest does not exist in the slot");
			if (quest.Id != questId)
				throw new Exception("incorrect quest");

			if(quest.Flags != quest.GetEndFlags())
				throw new Exception("cant complete the quest yet");

			var npcPosX = npcData[quest.GetEndNpcId()].PosX;
			var npcPosY = npcData[quest.GetEndNpcId()].PosY;

			if (posData.Instance?.MapId != (MapId)quest.GetEndMapId())
				throw new Exception("char not in correct instance");
			if (!posData.Movement.VerifyDistanceToNpc(npcPosX, npcPosY))
				throw new Exception("char too far away from npc");

			var questReward = quest.GetQuestReward();

			Item? itemReward = null;

            if (questReward.RewardItemIdx > 0)
            {
				itemReward = Item.GenerateReward((UInt32)questReward.RewardItemIdx, client.Character.Style.BattleStyleNum, choice);
            }

			if(itemReward != null)
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

			_activeQuests.Remove(slot);
			_completedQuests[questId] = true;

			return exp;
		}

		internal Quest ProgressQuest(UInt16 questId, Location posData, Dictionary<Int32, NpcData> npcData, List<QuestAction> questActions)
		{
			Quest? quest = null;
			foreach(var it in _activeQuests.Values)
			{
				if (it.Id == questId)
					quest = it;
			}
			if (quest == null)
				throw new Exception("Quest not found");

			if(quest.Started == false)
				throw new Exception("Quest not started");

			if (!quest.HasNpcActionSet())
				throw new Exception("Quest is not progressable that way");

			foreach(var it in questActions)
			{
				var actionSet = quest.GetActionSet(it.ChoiceId);

				if (actionSet == null)
					throw new Exception("bad action");

				uint actIdx = (uint)actionSet.ActIdx;
				Debug.Assert(actIdx == it.ChoiceId);
				var expectedIdx = quest.GetExpectedChoice();

				if(actIdx != expectedIdx)
					throw new Exception("action out of order");

				if (posData.Instance?.MapId != (MapId)actionSet.ActNpc[0])
					throw new Exception("char not in correct instance");

				var npcPosX = npcData[actionSet.ActNpc[1]].PosX;
				var npcPosY = npcData[actionSet.ActNpc[1]].PosY;
				if (!posData.Movement.VerifyDistanceToNpc(npcPosX, npcPosY))
					throw new Exception("char too far away from npc");

				quest.AddFlag(actIdx, (ushort)(1 << actionSet.Order));
			}

			
			return quest;
		}
	}
}
