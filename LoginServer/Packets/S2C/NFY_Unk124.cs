using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class NFY_Unk124 : PacketS2C
	{

		public NFY_Unk124() : base((UInt16)Opcode.UNK124)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, 0);
			PacketWriter.WriteUInt32(data, 100);
			PacketWriter.WriteUInt32(data, 200);
			PacketWriter.WriteUInt32(data, 300);
			PacketWriter.WriteUInt32(data, 400);
			PacketWriter.WriteByte(data, 1);
			PacketWriter.WriteUInt32(data, 500);
			PacketWriter.WriteUInt32(data, 600);
			PacketWriter.WriteUInt32(data, 700);
			PacketWriter.WriteUInt32(data, 800);
		}
	}
}
