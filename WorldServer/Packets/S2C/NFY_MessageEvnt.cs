using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_MessageEvnt : PacketS2C
	{
		Int32 _charId;
		Byte _remark;
		UInt16 _messageBlockLength, _messageLength;
		Byte _data1, _data2;
		MsgType _messageType;
		string _message;
		Byte _memoCount, _itemCount, _u2;

		public NFY_MessageEvnt(Int32 charId, MsgType messageType, String message) : base((UInt16)Opcode.NFY_MESSAGEEVNT)
		{
			_charId = charId;
			_messageType = messageType;
			_message = message;
			_remark = 1;
			_messageLength = (UInt16)(message.Length + 3);
			_messageBlockLength = (UInt16)(3 + 3 + 2 + _message.Length);
			_data1 = 0xFE;
			_data2 = 0xFE;
			_memoCount = 0;
			_itemCount = 0;
			_u2 = 0;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _charId);
			PacketWriter.WriteByte(data, _remark);
			PacketWriter.WriteUInt16(data, _messageBlockLength);
			PacketWriter.WriteUInt16(data, _messageLength);
			PacketWriter.WriteByte(data, _data1);
			PacketWriter.WriteByte(data, _data2);
			PacketWriter.WriteByte(data, (Byte)_messageType);
			PacketWriter.WriteString(data, _message);
			PacketWriter.WriteByte(data, _memoCount);
			PacketWriter.WriteByte(data, _itemCount);
			PacketWriter.WriteByte(data, _u2);
		}
	}
}
