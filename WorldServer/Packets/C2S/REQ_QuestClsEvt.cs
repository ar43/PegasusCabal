using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.CharData.Quests;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_QuestClsEvt : PacketC2S<Client>
	{
		public REQ_QuestClsEvt(Queue<byte> data) : base((UInt16)Opcode.CSC_QUESTCLSEVT, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 questId;
			UInt16 slot;
			UInt16 itemRewardChoice;
			UInt16 itemRewardInvSlot;



			try
			{
				questId = PacketReader.ReadUInt16(_data);
				Quest temp = new Quest(questId);
				var itemNeededCnt = 0;
				bool itemReward = temp.QuestInfoMain.QuestReward?.RewardItemIdx > 0;

				if (temp.QuestInfoMain.MissionItem != null)
					itemNeededCnt = temp.QuestInfoMain.MissionItem.Length;

				slot = PacketReader.ReadUInt16(_data);

				if (itemNeededCnt > 0)
				{
					for (int i = 0; i < itemNeededCnt; i++)
						_ = PacketReader.ReadUInt16(_data); // TODO - slots for various needed items
				}

				if(itemReward)
				{
					itemRewardChoice = PacketReader.ReadUInt16(_data);
					itemRewardInvSlot = PacketReader.ReadUInt16(_data);
				}
				else
				{
					itemRewardChoice = 0;
					itemRewardInvSlot = 0;
				}
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharQuest.OnQuestEnd(client, questId, slot, itemRewardChoice, itemRewardInvSlot));

			return true;
		}
	}
}
