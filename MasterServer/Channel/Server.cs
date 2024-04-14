namespace MasterServer.Channel
{
	internal class Server
	{
		public int Id { get; set; }
		public int Flags { get; set; }
		public List<Channel> Channels;

		public Server(Int32 id, Int32 flags, List<Channel> channels)
		{
			Id = id;
			Flags = flags;
			Channels = channels;
		}
	}
}
