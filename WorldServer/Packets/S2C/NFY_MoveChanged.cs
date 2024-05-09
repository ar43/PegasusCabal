using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_MoveChanged : PacketS2C
	{
		UInt32 _charId, _timestamp;
		UInt16 _fromX, _fromY, _toX, _toY;

		public NFY_MoveChanged(UInt32 charId, UInt32 timestamp, UInt16 fromX, UInt16 fromY, UInt16 toX, UInt16 toY) : base((UInt16)Opcode.NFY_MOVECHANGED)
		{
			_charId = charId;
			_timestamp = timestamp;
			_fromX = fromX;
			_fromY = fromY;
			_toX = toX;
			_toY = toY;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _charId);
			PacketWriter.WriteUInt32(data, _timestamp);
			PacketWriter.WriteUInt16(data, _fromX);
			PacketWriter.WriteUInt16(data, _fromY);
			PacketWriter.WriteUInt16(data, _toX);
			PacketWriter.WriteUInt16(data, _toY);
		}
	}
}
