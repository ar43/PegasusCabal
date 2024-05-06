using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using LibPegasus.Packets;
using WorldServer.Enums;

namespace WorldServer.Packets.C2S
{
	internal class REQ_SubPasswordCheckRequest : PacketC2S<Client>
	{
		public REQ_SubPasswordCheckRequest(Queue<byte> data) : base((UInt16)Opcode.CSC_SUBPASSWORDCHECKREQUEST, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => CharSelect.OnSubpasswordCheckRequest(x));

			return true;
		}
	}
}
