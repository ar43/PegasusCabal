using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_ItemLooting : PacketS2C
	{
		byte _result;
		uint _itemKind;
		uint _itemOption;
		int _slot;
		UInt16 _u0;

		public RSP_ItemLooting(Byte result, UInt32 itemKind, UInt32 itemOption, Int32 slot) : base((UInt16)Opcode.CSC_ITEMLOOTING)
		{
			_result = result;
			_itemKind = itemKind;
			_itemOption = itemOption;
			_slot = slot;
			_u0 = 0;
		}

		public RSP_ItemLooting(Byte result) : base((UInt16)Opcode.CSC_ITEMLOOTING)
		{
			_result = result;
			_itemKind = 0;
			_itemOption = 0;
			_slot = 0;
			_u0 = 0;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _result);
			PacketWriter.WriteUInt32(data, _itemKind);
			PacketWriter.WriteUInt32(data, _itemOption);
			PacketWriter.WriteInt32(data, _slot);
			PacketWriter.WriteUInt16(data, _u0);

		}
	}
}
