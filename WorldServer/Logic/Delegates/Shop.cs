using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Shop
	{
		internal static void OnAllPoolRequest(Client client)
		{
			if(client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}
			var rsp = new RSP_NpcShopPoolIdList(client.World.ShopPoolManager);
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
