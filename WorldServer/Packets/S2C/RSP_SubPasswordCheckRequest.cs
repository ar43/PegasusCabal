using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_SubPasswordCheckRequest : PacketS2C
	{
		private bool _pinRequired;

		public RSP_SubPasswordCheckRequest(bool pinRequired) : base((UInt16)Opcode.CSC_SUBPASSWORDCHECKREQUEST)
		{
			_pinRequired = pinRequired;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteBool(data, _pinRequired);
			PacketWriter.WriteNull(data, 3);
		}
	}
}
