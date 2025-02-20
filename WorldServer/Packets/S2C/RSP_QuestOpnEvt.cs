﻿using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_QuestOpnEvt : PacketS2C
	{
		Byte _result;

		public RSP_QuestOpnEvt(Byte result) : base((UInt16)Opcode.CSC_QUESTOPNEVT)
		{
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
		}
	}
}
