using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Cash
	{
		internal static void OnQueryCashItems(Client client)
		{
			//TODO: actually load 
			if (client.Account != null && client.Account.CashInventory != null)
			{
				var packet = new RSP_QueryCashItem(client.Account.CashInventory.Count(), client.Account.CashInventory.Serialize());
				client.PacketManager.Send(packet);
			}
			else
			{
				client.Disconnect("bad OnQueryCashItems", Enums.ConnState.ERROR);
			}
		}
	}
}
