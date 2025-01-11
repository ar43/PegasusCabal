using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_ItemDrop : PacketC2S<Client>
	{
		public REQ_ItemDrop(Queue<byte> data) : base((UInt16)Opcode.CSC_ITEMDROP, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			int fromType;
			int fromSlot;

			try
			{
				fromType = PacketReader.ReadInt32(_data);
				fromSlot = PacketReader.ReadInt32(_data);
				_ = PacketReader.ReadInt16(_data);
				_ = PacketReader.ReadInt16(_data);
				_ = PacketReader.ReadInt32(_data);
				_ = PacketReader.ReadInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharAction.OnItemDrop(client, (ItemMoveType)fromType, fromSlot));

			return true;
		}
	}
}
