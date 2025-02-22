using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Shop
	{
		internal static void OnAllPoolRequest(Client client)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}
			var rsp = new RSP_NpcShopPoolIdList(client.World.ShopPoolManager);
			client.PacketManager.Send(rsp);
		}

		internal static void OnItemBuy(Client client, Byte npcId, UInt16 shopId, UInt16 slotId, Int32 itemKind, Int32 itemOption, UInt16 slotId2, Int32 destinationSlot)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			var pool = client.World.ShopPoolManager.GetPool(shopId);

			if (slotId2 != slotId)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "Wrong slot2id");
				return;
			}

			if (pool.NpcId != npcId)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "wrong NPC id");
				return;
			}

			if ((int)client.Character.Location.Instance.MapId != pool.WorldId)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "wrong map id");
				return;
			}

			var item = pool.GetItem(slotId);
			var inv = client.Character.Inventory;

			if (item == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "item not found");
				return;
			}

			if(inv.Alz < (ulong)item.AlzPrice)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "insufficient alz");
				return;
			}

			if(item.ItemKind != itemKind || item.ItemOpt != itemOption)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "item info mismatch");
				return;
			}

			if(item.DurationIdx != 0)
			{
				throw new NotImplementedException();
			}

			if (!inv.AddItem((UInt16)destinationSlot, new CharData.Items.Item((UInt32)item.ItemKind, (UInt32)item.ItemOpt, 0, 0)))
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "can't add item");
				return;
			}

			inv.RemoveAlz((ulong)item.AlzPrice);

			var rsp = new RSP_ItemBuyings(0, item.ItemKind, item.ItemOpt);
			client.PacketManager.Send(rsp);
		}

		internal static void OnPoolRequest(Client client, UInt16 poolId)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			var rsp = new RSP_NpcShopPool(client.World.ShopPoolManager.GetPool(poolId));
			client.PacketManager.Send(rsp);
		}

		internal static void OnSyncRequest(Client client)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}
			var rsp = new NFY_NpcShopSync(0);
			client.PacketManager.Send(rsp);
		}
	}
}
