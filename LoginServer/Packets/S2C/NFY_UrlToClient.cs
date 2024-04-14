using LibPegasus.Packets;
using LoginServer.Enums;

namespace LoginServer.Packets.S2C
{
	internal class NFY_UrlToClient : PacketS2C
	{
		public NFY_UrlToClient() : base((UInt16)Opcode.URLTOCLIENT)
		{
		}

		public override void WritePayload()
		{
			// no idea what this is, just copying whatever actual EP8 server sends
			PacketWriter.WriteUInt16(_data, 22);
			PacketWriter.WriteUInt16(_data, 20);
			PacketWriter.WriteNull(_data, 5 * 4);
		}
	}
}
