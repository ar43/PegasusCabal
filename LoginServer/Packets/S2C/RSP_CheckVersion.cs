using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class RSP_CheckVersion : PacketS2C
	{
		private UInt32 _clientVersion;

		public RSP_CheckVersion(UInt32 clientVersion) : base((UInt16)Opcode.CHECKVERSION)
		{
			_clientVersion = clientVersion;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _clientVersion);
			PacketWriter.WriteUInt32(data, 0);
			PacketWriter.WriteUInt32(data, 0);
			PacketWriter.WriteUInt32(data, 0);

		}
	}
}
