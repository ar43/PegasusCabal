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
	internal class RSP_UseStatBons : PacketS2C
	{
		byte _result;

		public RSP_UseStatBons(Byte result) : base((UInt16)Opcode.CSC_USESTATBONS)
		{
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
		}
	}
}
