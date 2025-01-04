using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.CharData.Quests;
using WorldServer.Packets.C2S.PacketSpecificData;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class CharQuest
	{
		internal static void OnQuestNpcInteract(Client client, UInt16 questId, UInt32 setId, Byte maybeSlot, List<QuestAction> questActions)
		{
			if (client.Character?.Location.Instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			if(questActions.Count == 0)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "empty questActions");
				return;
			}

			//maybe maybeSlot is slot

			if (maybeSlot != 0)
			{
				throw new Exception("What");
			}
			Quest? quest;
			try
			{
				var npcData = client.Character.Location.Instance.MapData.NpcData;
				var posData = client.Character.Location;
				quest = client.Character.QuestManager.ProgressQuest(questId, posData, npcData, questActions);
			}
			catch (Exception)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "quest desync");
				return;
			}

			var packet_rsp = new RSP_QuestNpcActin(questId, quest.Flags, setId);
			client.PacketManager.Send(packet_rsp);
		}

		internal static void OnQuestStart(Client client, UInt16 questId, Byte slot)
		{
			if (client.Character == null || client.Character.Location == null || client.Character.Location.Instance.MapData.NpcData == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			try
			{
				var npcData = client.Character.Location.Instance.MapData.NpcData;
				var posData = client.Character.Location;
				client.Character.QuestManager.StartQuest(questId, slot, posData, npcData);
			}
			catch (Exception)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "quest desync");
				return;
			}

			var packet_rsp = new RSP_QuestOpnEvt(1); //TODO: send 0 if bad
			client.PacketManager.Send(packet_rsp);
		}
	}
}
