using LibPegasus.Logic.Delegates;
using LibPegasus.Logic;
using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibPegasus.Enums;

namespace LibPegasus.Packets.C2S
{
	internal class REQ_PublicKey : PacketC2S<Client>
	{
		public REQ_PublicKey(Queue<byte> data) : base((UInt16)Opcode.PUBLICKEY, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => Connection.OnPublicKeyRequest(x));

			return true;
		}
	}
}
