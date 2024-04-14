using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
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
