using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using WorldServer.Enums;
using LibPegasus.Packets;

namespace WorldServer.Packets.C2S
{
	internal class REQ_ChargeInfo : PacketC2S<Client>
	{
		public REQ_ChargeInfo(Queue<byte> data) : base((UInt16)Opcode.CHARGEINFO, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => CharSelect.OnChargeInfoRequest(x));

			return true;
		}
	}
}
