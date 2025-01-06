using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using WorldServer.Enums;

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
			UInt16 choice;
			UInt16 invSlot;

			try
			{
				questId = PacketReader.ReadUInt16(_data);
				slot = PacketReader.ReadUInt16(_data);
				choice = PacketReader.ReadUInt16(_data);
				invSlot = PacketReader.ReadUInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharQuest.OnQuestEnd(client, questId, slot, choice, invSlot));

			return true;
		}
	}
}
