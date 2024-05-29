using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_DelMobsList : PacketS2C
	{
		ObjectIndexData _oid;
		DelObjectType _delType;

		public NFY_DelMobsList(ObjectIndexData oid, DelObjectType delType) : base((UInt16)Opcode.NFY_DELMOBSLIST)
		{
			_oid = oid;
			_delType = delType;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _oid.ObjectId);
			PacketWriter.WriteByte(data, _oid.WorldIndex);
			PacketWriter.WriteByte(data, (Byte)_oid.ObjectType);
			PacketWriter.WriteByte(data, (byte)_delType);
		}
	}
}
