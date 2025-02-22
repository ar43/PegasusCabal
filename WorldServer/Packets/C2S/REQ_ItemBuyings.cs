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
	internal class REQ_ItemBuyings : PacketC2S<Client>
	{
		public REQ_ItemBuyings(Queue<byte> data) : base((UInt16)Opcode.CSC_ITEMBUYINGS, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte npcId;
			ushort shopId, slotId, slotId2;
			int itemKind, itemOption, u1, u2, u3, destinationSlot;


			try
			{
				npcId = PacketReader.ReadByte(_data);
				shopId = PacketReader.ReadUInt16(_data);
				slotId = PacketReader.ReadUInt16(_data);
				itemKind = PacketReader.ReadInt32(_data);
				itemOption = PacketReader.ReadInt32(_data);
				slotId2 = PacketReader.ReadUInt16(_data);
				u1 = PacketReader.ReadInt32(_data);
				u2 = PacketReader.ReadInt32(_data);
				u3 = PacketReader.ReadInt32(_data);
				destinationSlot = PacketReader.ReadInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Shop.OnItemBuy(client, npcId, shopId, slotId, itemKind, itemOption, slotId2, destinationSlot));

			return true;
		}
	}
}
