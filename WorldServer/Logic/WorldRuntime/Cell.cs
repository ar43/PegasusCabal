using WorldServer.Logic.WorldRuntime.InstanceRuntime;

namespace WorldServer.Logic.WorldRuntime
{
	internal class Cell
	{
		public Cell()
		{
			LocalClients = new();
			LocalMobs = new();
		}

		public HashSet<Client> LocalClients { get; private set; }
		public HashSet<Mob> LocalMobs { get; private set; }
	}
}
