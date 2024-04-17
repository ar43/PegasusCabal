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
	internal class REQ_ServerEnv : PacketC2S<Client>
	{
		public REQ_ServerEnv(Queue<byte> data) : base((UInt16)Opcode.SERVERENV, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => CharSelect.OnGetServerEnv(x));

			return true;
		}
	}
}
