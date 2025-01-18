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
	internal class REQ_QuestDungeonEnd : PacketC2S<Client>
	{
		public REQ_QuestDungeonEnd(Queue<byte> data) : base((UInt16)Opcode.CSC_QUESTDUNGEONEND, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 slotId;
			Byte success;
			DungeonEndCause cause;
			Byte npcId;

			try
			{
				slotId = PacketReader.ReadUInt16(_data);
				success = PacketReader.ReadByte(_data);
				cause = (DungeonEndCause)PacketReader.ReadByte(_data);
				npcId = PacketReader.ReadByte(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Dungeoneering.OnDungeonEnd(client, slotId, success, cause, npcId));

			return true;
		}
	}
}
