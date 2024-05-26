using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_ItemMove : PacketC2S<Client>
	{
		public REQ_ItemMove(Queue<byte> data) : base((UInt16)Opcode.CSC_ITEMMOVE, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			int fromType;
			int fromSlot;
			int toType;
			int toSlot;

			try
			{
				fromType = PacketReader.ReadInt32(_data);
				fromSlot = PacketReader.ReadInt32(_data);
				toType = PacketReader.ReadInt32(_data);
				toSlot = PacketReader.ReadInt32(_data);
				_ = PacketReader.ReadInt16(_data);
				_ = PacketReader.ReadInt16(_data);
				_ = PacketReader.ReadInt32(_data);
				_ = PacketReader.ReadInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Inventory.OnItemMove(client, fromType, fromSlot, toType, toSlot));

			return true;
		}
	}
}
