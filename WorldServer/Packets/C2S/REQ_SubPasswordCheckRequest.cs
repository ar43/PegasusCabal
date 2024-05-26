using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

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
