using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Interface
	{
		internal static void OnAutoStat(Client client, Int32 str, Int32 dex, Int32 intelligence)
		{
			throw new NotImplementedException();
		}

		internal static void OnStatSpend(Client client, Byte stat)
		{
			throw new NotImplementedException();
		}

		internal static void OnUpdateHelpInfo(Client client)
		{
			var packet = new RSP_UpdateHelpInfo(1);
			client.PacketManager.Send(packet);
		}
	}
}
