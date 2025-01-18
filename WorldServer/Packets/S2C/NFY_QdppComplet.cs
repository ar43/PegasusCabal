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
	internal class NFY_QdppComplet : PacketS2C
	{
		Byte _seqIdx;

		public NFY_QdppComplet(Byte seqIdx) : base((UInt16)Opcode.NFY_QDPPCOMPLET)
		{
			_seqIdx = seqIdx;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _seqIdx);
		}
	}
}
