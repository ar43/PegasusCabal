using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_ChangeStyle : PacketS2C
	{
		byte _result;
		public RSP_ChangeStyle(byte result) : base((UInt16)Opcode.CSC_CHANGESTYLE)
		{
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
		}
	}
}
