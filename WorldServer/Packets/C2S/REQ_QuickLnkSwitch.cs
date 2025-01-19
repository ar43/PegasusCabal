using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using WorldServer.Enums;

namespace WorldServer.Packets.C2S
{
	internal class REQ_QuickLnkSwitch : PacketC2S<Client>
	{
		public REQ_QuickLnkSwitch(Queue<byte> data) : base((UInt16)Opcode.CSC_QUICKLNKSWITCH, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			Int16 quickSlot1, quickSlot2;

			try
			{
				quickSlot1 = PacketReader.ReadInt16(_data);
				quickSlot2 = PacketReader.ReadInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Interface.OnQuickLinkSwitch(client, quickSlot1, quickSlot2));

			return true;
		}
	}
}
