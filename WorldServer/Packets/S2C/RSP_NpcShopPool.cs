using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.ShopRuntime;

namespace WorldServer.Packets.S2C
{
	internal class RSP_NpcShopPool : PacketS2C
	{
		ShopPool _pool;
		public RSP_NpcShopPool(ShopPool pool) : base((UInt16)Opcode.CSC_NPCSHOPPOOL)
		{
			_pool = pool;
		}

		public override void WritePayload(Deque<byte> data)
		{
			var count = _pool.Count();
			PacketWriter.WriteUInt16(data, (UInt16)_pool.PoolId);
			PacketWriter.WriteUInt16(data, (UInt16)count);

			foreach (var item in _pool.Items)
			{
				var price = item.Value.AlzPrice == 0 ? item.Value.CashPrice : item.Value.AlzPrice;
				PacketWriter.WriteUInt16(data, (UInt16)item.Key);
				PacketWriter.WriteInt32(data, item.Value.ItemKind);
				PacketWriter.WriteInt32(data, item.Value.ItemOpt);
				PacketWriter.WriteUInt16(data, (UInt16)item.Value.DurationIdx); //might be int32
				PacketWriter.WriteNull(data, 4); //unk
				PacketWriter.WriteInt8(data, (SByte)item.Value.Reputation);
				PacketWriter.WriteInt8(data, (SByte)item.Value.MaxReputation);
				PacketWriter.WriteNull(data, 6); //unk
				PacketWriter.WriteInt32(data, price);
				PacketWriter.WriteByte(data, (Byte)item.Value.Marker);
				PacketWriter.WriteNull(data, 3); //unk
			}
		}
	}
}
