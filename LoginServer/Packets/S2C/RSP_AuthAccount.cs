using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class RSP_AuthAccount : PacketS2C
	{
		private UInt32 _status;

		public RSP_AuthAccount(UInt32 status) : base((UInt16)Opcode.AUTHACCOUNT)
		{
			_status = status;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _status);
			PacketWriter.WriteUInt32(data, 1);
			PacketWriter.WriteUInt32(data, 0);
		}
	}
}
