using Google.Protobuf.WellKnownTypes;

namespace MasterServer.Channel
{
	public class Channel
	{
		public UInt32 ServerId { get; private set; }
		public UInt32 ChannelId { get; private set; }
		public string Ip { get; private set; }
		public UInt32 Port { get; private set; }

		public DateTime LastUpdate {  get; private set; }

		private readonly double TIMEOUT = 10.0;

		public Channel(UInt32 serverId, UInt32 channelId, string ip, UInt32 port)
		{
			ServerId = serverId;
			ChannelId = channelId;
			Ip = ip;
			Port = port;
			LastUpdate = DateTime.UtcNow;
		}

		public void Update(string ip, UInt32 port)
		{
			LastUpdate = DateTime.UtcNow;
			Ip = ip;
			Port = port;
		}

		public bool IsEqual(UInt32 serverId, UInt32 channelId)
		{
			if(serverId == ServerId && channelId == ChannelId)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsExpired()
		{
			if(DateTime.UtcNow.Ticks - LastUpdate.Ticks >= TimeSpan.FromSeconds(TIMEOUT).Ticks)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Print()
		{
			Serilog.Log.Debug($"SID: {ServerId} | CID: {ChannelId} | IP: {Ip} | Port: {Port}");
		}
	}
}
