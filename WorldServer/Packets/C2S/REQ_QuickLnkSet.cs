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
	internal class REQ_QuickLnkSet : PacketC2S<Client>
	{
		public REQ_QuickLnkSet(Queue<byte> data) : base((UInt16)Opcode.CSC_QUICKLNKSET, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			Int16 quickSlot, skillSlot;

			try
			{
				quickSlot = PacketReader.ReadInt16(_data);
				skillSlot = PacketReader.ReadInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Interface.OnQuickLinkSet(client, quickSlot, skillSlot));

			return true;
		}
	}
}
