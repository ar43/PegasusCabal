using WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime;

namespace WorldServer.Logic.WorldRuntime
{
	internal class Cell
	{
		public Cell()
		{
			LocalClients = new();
			LocalMobs = new();
			LocalGroundItems = new();
		}

		public HashSet<Client> LocalClients { get; private set; }
		public HashSet<Mob> LocalMobs { get; private set; }
		public HashSet<GroundItem> LocalGroundItems { get; private set; }
	}
}
