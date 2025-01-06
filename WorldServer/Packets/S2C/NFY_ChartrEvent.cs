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
	internal class NFY_ChartrEvent : PacketS2C
	{
		CharEvent evt;
		int charId;

		public NFY_ChartrEvent(CharEvent evt, Int32 charId) : base((UInt16)Opcode.NFY_CHARTREVENT)
		{
			this.evt = evt;
			this.charId = charId;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, (Byte)evt);
			PacketWriter.WriteInt32(data, charId);
		}
	}
}
