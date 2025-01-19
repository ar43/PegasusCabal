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
	internal class REQ_AuraExchang : PacketC2S<Client>
	{
		public REQ_AuraExchang(Queue<byte> data) : base((UInt16)Opcode.CSC_AURAEXCHANG, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte auraId;

			try
			{
				auraId = PacketReader.ReadByte(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Miscellaneous.OnAuraExchang(client, auraId));

			return true;
		}
	}
}
