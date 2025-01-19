
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Miscellaneous
	{
		internal static void OnAuraExchang(Client client, Byte auraId)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			//TODO: verification
			client.Character.Style.SetAura(auraId);
			var rsp = new RSP_AuraExchang(auraId);
			//TODO: 1 or aura code
			client.PacketManager.Send(rsp);
		}

		internal static void OnHeartbeatResponse(Client client)
		{
			client.TimerHeartbeatTimeout = null;
		}
	}
}
