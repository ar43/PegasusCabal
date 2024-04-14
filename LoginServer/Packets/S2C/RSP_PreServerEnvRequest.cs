using LibPegasus.Packets;
using LoginServer.Enums;

namespace LoginServer.Packets.S2C
{
	internal class RSP_PreServerEnvRequest : PacketS2C
	{
		public RSP_PreServerEnvRequest() : base((UInt16)Opcode.PRESERVERENVREQUEST)
		{
		}

		public override void WritePayload()
		{
			PacketWriter.WriteNull(_data, 4113);
		}
	}
}
