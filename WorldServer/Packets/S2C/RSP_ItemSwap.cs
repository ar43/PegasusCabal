using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_ItemSwap : PacketS2C
	{
		byte _result;
		public RSP_ItemSwap(byte result) : base((UInt16)Opcode.CSC_ITEMSWAP)
		{
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
		}
	}
}
