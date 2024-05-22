using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_ItemMove : PacketS2C
	{
		byte _result;
		public RSP_ItemMove(byte result) : base((UInt16)Opcode.CSC_ITEMMOVE)
		{
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
			PacketWriter.WriteNull(data, 4);
		}
	}
}
