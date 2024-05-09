using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class REQ_Heartbeat : PacketS2C
	{
		public REQ_Heartbeat() : base((UInt16)Opcode.CSC_HEARTBEAT)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
		}
	}
}
