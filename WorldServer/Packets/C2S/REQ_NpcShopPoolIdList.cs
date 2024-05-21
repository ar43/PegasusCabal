using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_NpcShopPoolIdList : PacketC2S<Client>
	{
		public REQ_NpcShopPoolIdList(Queue<byte> data) : base((UInt16)Opcode.CSC_NPCSHOPPOOLIDLIST, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((client) => Shop.OnAllPoolRequest(client));

			return true;
		}
	}
}
