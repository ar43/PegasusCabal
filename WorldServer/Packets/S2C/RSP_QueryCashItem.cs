using LibPegasus.Packets;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_QueryCashItem : PacketS2C
	{
		Int32 _count;
		byte[] _serializedItemData;

		public RSP_QueryCashItem(Int32 count, Byte[] serializedItemData) : base((UInt16)Opcode.CSC_QUERYCASHITEM)
		{
			_count = count;
			_serializedItemData = serializedItemData;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteInt32(_data, _count);
			if(_count > 0)
			{
				PacketWriter.WriteArray(_data, _serializedItemData);
			}
		}
	}
}
