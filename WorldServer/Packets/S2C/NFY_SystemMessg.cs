using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_SystemMessg : PacketS2C
	{
		Byte _messageType;
		ushort _messageLength;
		string _sysMsg;

		public NFY_SystemMessg(Byte messageType, UInt16 messageLength, String sysMsg) : base((UInt16)Opcode.NFY_SYSTEMMESSG)
		{
			_messageType = messageType;
			_messageLength = messageLength;
			_sysMsg = sysMsg;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _messageType);
			PacketWriter.WriteUInt16(data, _messageLength);
			PacketWriter.WriteString(data, _sysMsg);
			//PacketWriter.WriteByte(data, 0);
		}
	}
}
