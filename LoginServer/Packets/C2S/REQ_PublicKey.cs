using LoginServer.Logic.Delegates;
using LoginServer.Logic;
using LoginServer.Opcodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginServer.Enums;

namespace LoginServer.Packets.C2S
{
	internal class REQ_PublicKey : PacketC2S
	{
		public REQ_PublicKey(Queue<byte> data) : base(Opcode.PUBLICKEY, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => Connection.OnPublicKeyRequest(x));

			return true;
		}
	}
}
