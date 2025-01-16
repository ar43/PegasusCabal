using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_NewLoudMsg : PacketS2C
	{
		Int32 _msgType;
		Int32 _charId;
		Byte _nation;
		UInt16 _messageBlockLength;
		string _message, _name;
		Byte _memoCount, _itemCount, _u2;

		public NFY_NewLoudMsg(Int32 msgType, Int32 charId, Byte nation, String message, String name) : base((UInt16)Opcode.NFY_NEWLOUDMSG)
		{
			_msgType = msgType;
			_charId = charId;
			_message = message;
			_nation = nation;
			_messageBlockLength = (UInt16)(3 + message.Length + 2 + name.Length + 2);
			_memoCount = 0;
			_itemCount = 0;
			_u2 = 0;
			_name = name;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _msgType);
			PacketWriter.WriteInt32(data, _charId);
			PacketWriter.WriteByte(data, _nation);
			PacketWriter.WriteUInt16(data, _messageBlockLength);
			PacketWriter.WriteUInt16(data, (UInt16)_name.Length);
			PacketWriter.WriteString(data, _name);
			PacketWriter.WriteUInt16(data, (UInt16)_message.Length);
			PacketWriter.WriteString(data, _message);
			PacketWriter.WriteByte(data, _memoCount);
			PacketWriter.WriteByte(data, _itemCount);
			PacketWriter.WriteByte(data, _u2);
		}
	}
}
