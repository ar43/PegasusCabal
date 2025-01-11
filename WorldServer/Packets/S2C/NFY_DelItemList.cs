using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_DelItemList : PacketS2C
	{
		ObjectIndexData _oid;
		DelObjectType _delType;

		public NFY_DelItemList(ObjectIndexData oid, DelObjectType delType) : base((UInt16)Opcode.NFY_DELITEMLIST)
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
