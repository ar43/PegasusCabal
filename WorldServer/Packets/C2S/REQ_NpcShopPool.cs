using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_NpcShopPool : PacketC2S<Client>
	{
		public REQ_NpcShopPool(Queue<byte> data) : base((UInt16)Opcode.CSC_NPCSHOPPOOL, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 poolId;

			try
			{
				poolId = PacketReader.ReadUInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Shop.OnPoolRequest(client, poolId));

			return true;
		}
	}
}
