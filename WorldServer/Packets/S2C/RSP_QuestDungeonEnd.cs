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
	internal class RSP_QuestDungeonEnd : PacketS2C
	{
		Int32 u0;
		Byte result;

		public RSP_QuestDungeonEnd(Int32 u0, Byte result) : base((UInt16)Opcode.CSC_QUESTDUNGEONEND)
		{
			this.u0 = u0;
			this.result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, u0);
			PacketWriter.WriteByte(data, result);
		}
	}
}
