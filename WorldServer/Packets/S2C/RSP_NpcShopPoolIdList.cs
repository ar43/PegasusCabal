using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.WorldRuntime.ShopRuntime;

namespace WorldServer.Packets.S2C
{
	internal class RSP_NpcShopPoolIdList : PacketS2C
	{
		ShopPoolManager _shopPoolManager;
		public RSP_NpcShopPoolIdList(ShopPoolManager shopPoolManager) : base((UInt16)Opcode.CSC_NPCSHOPPOOLIDLIST)
		{
			_shopPoolManager = shopPoolManager;
		}

		public override void WritePayload(Deque<byte> data)
		{
			Int16 count = (Int16)_shopPoolManager.Count();
			PacketWriter.WriteInt16(data, count);
			for(int i = 1; i <= count; i++)
			{
				var shop = _shopPoolManager.GetPool(i);
				PacketWriter.WriteByte(data, (Byte)shop.WorldId);
				PacketWriter.WriteByte(data, (Byte)shop.NpcId);
				PacketWriter.WriteNull(data, 2); //unk
				PacketWriter.WriteInt16(data, (Int16)shop.PoolId);
				PacketWriter.WriteInt16(data, (Int16)shop.Pool2Id);
				PacketWriter.WriteNull(data, 2); //unk
				PacketWriter.WriteByte(data, 1); //the 1 that is always present??
			}
		}
	}
}
