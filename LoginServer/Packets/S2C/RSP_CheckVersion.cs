using LibPegasus.Packets;
using LoginServer.Enums;

namespace LoginServer.Packets.S2C
{
	internal class RSP_CheckVersion : PacketS2C
	{
		private UInt32 _clientVersion;

		public RSP_CheckVersion(UInt32 clientVersion) : base((UInt16)Opcode.CHECKVERSION)
		{
			_clientVersion = clientVersion;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteUInt32(_data, _clientVersion);
			PacketWriter.WriteUInt32(_data, 0);
			PacketWriter.WriteUInt32(_data, 0);
			PacketWriter.WriteUInt32(_data, 0);

		}
	}
}
