using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_UpdateDatas : PacketS2C
	{
		UpdateType _type;
		ushort _s0;
		ushort _s1;

		public NFY_UpdateDatas(UpdateType type, UInt16 s0, UInt16 s2) : base((UInt16)Opcode.NFY_UPDATEDATAS)
		{
			this._type = type;
			this._s0 = s0;
			this._s1 = s2;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, (Byte)_type);
			PacketWriter.WriteNull(data, 14);
			PacketWriter.WriteUInt16(data, _s0);
			PacketWriter.WriteUInt16(data, _s1);
			PacketWriter.WriteUInt32(data, 0);
		}
	}
}
