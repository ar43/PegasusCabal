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
	internal class REQ_GetSvrTime : PacketC2S<Client>
	{
		public REQ_GetSvrTime(Queue<byte> data) : base((UInt16)Opcode.GETSVRTIME, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => CharSelect.OnGetSvrTime(x));

			return true;
		}
	}
}
