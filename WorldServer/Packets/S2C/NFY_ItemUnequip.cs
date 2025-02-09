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
	internal class NFY_ItemUnequip : PacketS2C
	{
		Int32 _charId;
		UInt16 _slot;

		public NFY_ItemUnequip(Int32 charId, UInt16 slot) : base((UInt16)Opcode.NFY_ITEMUNEQUIP)
		{
			_charId = charId;
			_slot = slot;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _charId);
			PacketWriter.WriteUInt16(data, _slot);
		}
	}
}
