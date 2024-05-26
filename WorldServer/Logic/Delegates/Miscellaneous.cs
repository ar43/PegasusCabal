namespace WorldServer.Logic.Delegates
{
	internal static class Miscellaneous
	{
		internal static void OnHeartbeatResponse(Client client)
		{
			client.TimerHeartbeatTimeout = null;
		}
	}
}
