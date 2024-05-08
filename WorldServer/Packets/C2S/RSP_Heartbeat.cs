using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class RSP_Heartbeat : PacketC2S<Client>
	{
		public RSP_Heartbeat(Queue<byte> data) : base((UInt16)Opcode.CSC_HEARTBEAT, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((client) => Miscellaneous.OnHeartbeatResponse(client));

			return true;
		}
	}
}
