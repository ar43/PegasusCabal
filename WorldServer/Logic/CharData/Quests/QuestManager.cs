using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;

namespace WorldServer.Logic.CharData.Quests
{
    internal class QuestManager
    {
		private Dictionary<int, Quest> _activeQuests = new();
		private BitArray _startedQuests;

		public QuestManager()
		{
			_startedQuests = new(1023 * 8);
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
