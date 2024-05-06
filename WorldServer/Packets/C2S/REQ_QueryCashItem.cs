using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_QueryCashItem : PacketC2S<Client>
	{
		public REQ_QueryCashItem(Queue<byte> data) : base((UInt16)Opcode.CSC_QUERYCASHITEM, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((client) => Cash.OnQueryCashItems(client));

			return true;
		}
	}
}
