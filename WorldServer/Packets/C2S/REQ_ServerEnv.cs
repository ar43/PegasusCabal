using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_ServerEnv : PacketC2S<Client>
	{
		public REQ_ServerEnv(Queue<byte> data) : base((UInt16)Opcode.CSC_SERVERENV, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => CharSelect.OnGetServerEnv(x));

			return true;
		}
	}
}
