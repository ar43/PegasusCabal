using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_Unk3383 : PacketC2S<Client>
	{
		public REQ_Unk3383(Queue<byte> data) : base((UInt16)Opcode.UNK3383, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => Connection.OnUnk3383(x));

			return true;
		}
	}
}
