using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_Unk994 : PacketS2C
	{

		public NFY_Unk994() : base((UInt16)Opcode.NFY_UNK994)
		{
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
		}
	}
}
