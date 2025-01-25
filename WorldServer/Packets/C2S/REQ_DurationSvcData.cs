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
	internal class REQ_DurationSvcData : PacketC2S<Client>
	{
		public REQ_DurationSvcData(Queue<byte> data) : base((UInt16)Opcode.CSC_DURATIONSVCDATA, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			try
			{
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Interface.OnPremiumDataRequest(client));

			return true;
		}
	}
}
