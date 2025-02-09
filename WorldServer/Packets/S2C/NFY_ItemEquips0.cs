using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_ItemEquips0 : PacketS2C
	{
		Int32 _charId;
		UInt32 _itemKind;
		UInt16 _slot;
		Int32 _u0 = 0;
		Byte _u1 = 0;

		public NFY_ItemEquips0(Int32 charId, UInt32 itemKind, UInt16 slot) : base((UInt16)Opcode.NFY_ITEMEQUIPS0)
		{
			_charId = charId;
			_itemKind = itemKind;
			_slot = slot;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _charId);
			PacketWriter.WriteUInt32(data, _itemKind);
			PacketWriter.WriteUInt16(data, _slot);
			PacketWriter.WriteInt32(data, _u0);
			PacketWriter.WriteByte(data, _u1);
		}
	}
}
