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
	internal class RSP_DungeonStart : PacketS2C
	{
		int _unknown;
		public RSP_DungeonStart(int unknown) : base((UInt16)Opcode.CSC_DUNGEONSTART)
		{
			_unknown = unknown;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _unknown);
		}
	}
}
