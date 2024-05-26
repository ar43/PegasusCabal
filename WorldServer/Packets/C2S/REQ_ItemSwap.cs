using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_ItemSwap : PacketC2S<Client>
	{
		public REQ_ItemSwap(Queue<byte> data) : base((UInt16)Opcode.CSC_ITEMSWAP, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			int fromType1;
			int fromSlot1;
			int toType1;
			int toSlot1;
			int fromType2;
			int fromSlot2;
			int toType2;
			int toSlot2;

			try
			{
				fromType1 = PacketReader.ReadInt32(_data);
				fromSlot1 = PacketReader.ReadInt32(_data);
				toType1 = PacketReader.ReadInt32(_data);
				toSlot1 = PacketReader.ReadInt32(_data);
				fromType2 = PacketReader.ReadInt32(_data);
				fromSlot2 = PacketReader.ReadInt32(_data);
				toType2 = PacketReader.ReadInt32(_data);
				toSlot2 = PacketReader.ReadInt32(_data);
				_ = PacketReader.ReadInt16(_data);
				_ = PacketReader.ReadInt16(_data);
				_ = PacketReader.ReadInt32(_data);
				_ = PacketReader.ReadInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Inventory.OnItemSwap(client, fromType1, fromSlot1, toType1, toSlot1, fromType2, fromSlot2, toType2, toSlot2));

			return true;
		}
	}
}
