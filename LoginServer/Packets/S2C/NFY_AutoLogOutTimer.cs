using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class NFY_AutoLogOutTimer : PacketS2C
	{
		UInt32 _disconnectTime;

		public NFY_AutoLogOutTimer(UInt32 disconnectTime) : base((UInt16)Opcode.AUTOLOGOUTTIMER)
		{
			_disconnectTime = disconnectTime;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _disconnectTime);
		}
	}
}
