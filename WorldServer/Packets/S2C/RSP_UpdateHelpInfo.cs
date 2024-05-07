using LibPegasus.Packets;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_UpdateHelpInfo : PacketS2C
	{
		byte _result;
		public RSP_UpdateHelpInfo(byte result) : base((UInt16)Opcode.CSC_UPDATEHELPINFO)
		{
			_result = result;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteByte(_data, _result);
		}
	}
}
