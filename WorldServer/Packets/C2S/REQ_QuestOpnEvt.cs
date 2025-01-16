using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_QuestOpnEvt : PacketC2S<Client>
	{
		public REQ_QuestOpnEvt(Queue<byte> data) : base((UInt16)Opcode.CSC_QUESTOPNEVT, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 questId;
			Byte slot;
			UInt16 unk;

			try
			{
				questId = PacketReader.ReadUInt16(_data);
				slot = PacketReader.ReadByte(_data);
				unk = PacketReader.ReadUInt16(_data);

				if (unk != 0xFFFF)
					throw new NotImplementedException("What is this?");
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharQuest.OnQuestStart(client, questId, slot));

			return true;
		}
	}
}
