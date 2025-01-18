using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_QuestDungeonEnd : PacketS2C
	{
		Int32 _charId;
		Byte _success;
		DungeonEndCause _cause;

		public NFY_QuestDungeonEnd(Int32 charId, Byte success, DungeonEndCause cause) : base((UInt16)Opcode.NFY_QUESTDUNGEONEND)
		{
			_charId = charId;
			_success = success;
			_cause = cause;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _charId);
			PacketWriter.WriteByte(data, _success);
			PacketWriter.WriteByte(data, (byte)_cause);
		}
	}
}
