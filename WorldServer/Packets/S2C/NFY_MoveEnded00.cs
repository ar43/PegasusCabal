using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_MoveEnded00 : PacketS2C
	{
		UInt32 _charId;
		UInt16 _x, _y;

		public NFY_MoveEnded00(UInt32 charId, UInt16 x, UInt16 y) : base((UInt16)Opcode.NFY_MOVEENDED00)
		{
			_charId = charId;
			_x = x;
			_y = y;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _charId);
			PacketWriter.WriteUInt16(data, _x);
			PacketWriter.WriteUInt16(data, _y);
		}
	}
}
