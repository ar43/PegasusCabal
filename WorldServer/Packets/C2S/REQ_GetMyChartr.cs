using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_GetMyChartr : PacketC2S<Client>
	{
		public REQ_GetMyChartr(Queue<byte> data) : base((UInt16)Opcode.GETMYCHARTR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((x) => CharSelect.OnCharacterRequest(x));

			return true;
		}
	}
}
