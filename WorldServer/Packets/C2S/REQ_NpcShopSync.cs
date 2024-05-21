using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_NpcShopSync : PacketC2S<Client>
	{
		public REQ_NpcShopSync(Queue<byte> data) : base((UInt16)Opcode.REQ_NPCSHOPSYNC, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((client) => Shop.OnSyncRequest(client));

			return true;
		}
	}
}
