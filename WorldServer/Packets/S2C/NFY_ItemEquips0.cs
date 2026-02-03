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
		UInt64 _itemKind;
		UInt64 _itemOption;
		UInt16 _slot;
		Int32 _u0 = 0;
		Byte _u1 = 0;

		public NFY_ItemEquips0(Int32 charId, UInt64 itemKind, UInt64 itemOption, UInt16 slot) : base((UInt16)Opcode.NFY_ITEMEQUIPS0)
		{
			_charId = charId;
			_itemKind = itemKind;
			_itemOption = itemOption;
			_slot = slot;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _charId);
			PacketWriter.WriteUInt64(data, _itemKind);
			PacketWriter.WriteUInt64(data, _itemOption);
			PacketWriter.WriteUInt16(data, _slot);
			PacketWriter.WriteInt32(data, _u0);
			PacketWriter.WriteByte(data, _u1);
		}
	}
}
