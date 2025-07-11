using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;

namespace WorldServer.Packets.C2S
{
	internal class REQ_ItemSelling : PacketC2S<Client>
	{
		public REQ_ItemSelling(Queue<byte> data) : base((UInt16)Opcode.CSC_ITEMSELLING, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte shopId;
			int u0;
			int u1;
			int inventorySlot;

			try
			{
				shopId = PacketReader.ReadByte(_data);
				u0 = PacketReader.ReadInt32(_data);
				u1 = PacketReader.ReadInt32(_data);
				inventorySlot = PacketReader.ReadInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Shop.OnItemSell(client, shopId, u0 ,u1, inventorySlot));

			return true;
		}
	}
}
