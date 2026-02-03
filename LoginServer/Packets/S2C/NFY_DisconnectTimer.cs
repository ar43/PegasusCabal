using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;

namespace LoginServer.Packets.S2C
{
	internal class NFY_DisconnectTimer : PacketS2C
	{
		UInt32 _disconnectTime;

		public NFY_DisconnectTimer(UInt32 disconnectTime) : base((UInt16)Opcode.DISCONNECTTIMER)
		{
			_disconnectTime = disconnectTime;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _disconnectTime);
			PacketWriter.WriteUInt32(data, 0);
			PacketWriter.WriteByte(data, 0);
		}
	}
}
