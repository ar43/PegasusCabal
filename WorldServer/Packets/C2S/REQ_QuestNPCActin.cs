using LibPegasus.Packets;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.CharData.Quests;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_QuestNPCActin : PacketC2S<Client>
	{
		public REQ_QuestNPCActin(Queue<byte> data) : base((UInt16)Opcode.CSC_QUESTNPCACTIN, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 questId;
			UInt32 setId;
			Byte maybeSlot;
			Byte numActions;
			List<QuestAction> questActions = new();

			try
			{
				questId = PacketReader.ReadUInt16(_data);
				setId = PacketReader.ReadUInt32(_data);
				maybeSlot = PacketReader.ReadByte(_data);
				numActions = PacketReader.ReadByte(_data);
				for (int i = 0; i < numActions; i++)
				{
					var action = new QuestAction();
					action.ChoiceId = PacketReader.ReadUInt32(_data);
					action.Param = PacketReader.ReadUInt16(_data);
					questActions.Add(action);
				}

				if (questId != setId)
				{
					Serilog.Log.Error("questId not equal to setId");
					Debug.Assert(false);
					return false;
				}
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharQuest.OnQuestNpcInteract(client, questId, setId, maybeSlot, questActions));

			return true;
		}
	}
}
