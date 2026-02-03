using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class RSP_Unk3383 : PacketS2C
	{

		public RSP_Unk3383() : base((UInt16)Opcode.UNK3383)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, 0);
			PacketWriter.WriteUInt32(data, 706);
			PacketWriter.WriteUInt16(data, 0);
		}
	}
}
