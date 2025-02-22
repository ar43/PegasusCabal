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
	internal class RSP_ItemBuyings : PacketS2C
	{
		int _result, _itemKind, _itemOption, _u0;
		ushort _u1;

		public RSP_ItemBuyings(Int32 result, Int32 itemKind, Int32 itemOption) : base((UInt16)Opcode.CSC_ITEMBUYINGS)
		{
			_result = result;
			_itemKind = itemKind;
			_itemOption = itemOption;
			_u0 = 0;
			_u1 = 0;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _result);
			PacketWriter.WriteInt32(data, _itemKind);
			PacketWriter.WriteInt32(data, _itemOption);
			PacketWriter.WriteInt32(data, _u0);
			PacketWriter.WriteUInt16(data, _u1);
		}
	}
}
