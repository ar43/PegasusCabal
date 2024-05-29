using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_MobsMoveBgn : PacketS2C
	{
		ObjectIndexData _id;
		UInt32 _timestamp;
		UInt16 _fromX, _fromY, _toX, _toY;

		public NFY_MobsMoveBgn(ObjectIndexData id, UInt32 timestamp, UInt16 fromX, UInt16 fromY, UInt16 toX, UInt16 toY) : base((UInt16)Opcode.NFY_MOBSMOVEBGN)
		{
			_id = id;
			_timestamp = timestamp;
			_fromX = fromX;
			_fromY = fromY;
			_toX = toX;
			_toY = toY;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _id.ObjectId);
			PacketWriter.WriteByte(data, _id.WorldIndex);
			PacketWriter.WriteByte(data, (Byte)_id.ObjectType);
			PacketWriter.WriteUInt32(data, _timestamp);
			PacketWriter.WriteUInt16(data, _fromX);
			PacketWriter.WriteUInt16(data, _fromY);
			PacketWriter.WriteUInt16(data, _toX);
			PacketWriter.WriteUInt16(data, _toY);
		}
	}
}
