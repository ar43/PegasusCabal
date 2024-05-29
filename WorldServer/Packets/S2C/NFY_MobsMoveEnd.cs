using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_MobsMoveEnd : PacketS2C
	{
		ObjectIndexData _id;
		UInt16 _x, _y;

		public NFY_MobsMoveEnd(ObjectIndexData id, UInt16 x, UInt16 y) : base((UInt16)Opcode.NFY_MOBSMOVEEND)
		{
			_id = id;
			_x = x;
			_y = y;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _id.ObjectId);
			PacketWriter.WriteByte(data, _id.WorldIndex);
			PacketWriter.WriteByte(data, (Byte)_id.ObjectType);
			PacketWriter.WriteUInt16(data, _x);
			PacketWriter.WriteUInt16(data, _y);
		}
	}
}
