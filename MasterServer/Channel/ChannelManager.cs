using LibPegasus.Enums;
using Shared.Protos;
using System.Threading.Channels;

namespace MasterServer.Channel
{
    public class ChannelManager
    {
		List<Server> _servers;
		public ChannelManager(IConfiguration configuration)
		{
			var serverData = configuration["ServerData"].Split(',').Select(s => Int32.Parse(s)).ToArray();
			_servers = new List<Server>();
			for (int i = 0; i < serverData.Length; i += 2) 
			{
				_servers.Add(new Server(serverData[i], serverData[i + 1], new List<Channel>()));
				Serilog.Log.Information($"Added server with id {serverData[i]}");
			}
		}

		public InfoCodeWorldHeartbeat Heartbeat(UInt32 serverId, UInt32 channelId, string ip, UInt32 port) 
		{
			lock(_servers)
			{
				var server = _servers.Find(x => x.Id == serverId);

                if (server != null)
                {
					foreach (Channel channel in server.Channels)
					{
						if (channel.IsEqual(serverId, channelId))
						{
							channel.Update(ip, port);
							return InfoCodeWorldHeartbeat.UPDATED;
						}
					}
					server.Channels.Add(new Channel(serverId, channelId, ip, port));
					return InfoCodeWorldHeartbeat.ADDED;
				}
				return InfoCodeWorldHeartbeat.SERVER_UNDEFINED;
			}
		}

		public void RemoveExpiredChannels()
		{
			lock (_servers)
			{
				foreach(Server server in _servers)
				{
					for (int i = server.Channels.Count - 1; i >= 0; i--)
					{
						if (server.Channels[i].IsExpired())
						{
							server.Channels.RemoveAt(i);
						}
					}
				}
			}
		}

		public List<ServerMsg> GetServerMsg(bool isLocalhost)
		{ 
			lock (_servers)
			{
				var outData = new List<ServerMsg>();
				foreach (Server server in _servers)
				{
					ServerMsg smsg = new ServerMsg();
					smsg.ServerId = (UInt32)server.Id;
					smsg.ServerFlag = (UInt32)server.Flags;
					smsg.ChannelCount = (UInt32)server.Channels.Count;
					var cmsgs = new List<ChannelMsg>();

					foreach(Channel channel in server.Channels)
					{
						var cmsg = new ChannelMsg();
						cmsg.ChannelId = channel.ChannelId;
						cmsg.UserCount = 0; // TODO
						cmsg.MaximumUserCount = 60; // TODO
						if (!isLocalhost)
							cmsg.Ip = channel.Ip;
						else
							cmsg.Ip = "127.0.0.1";
						cmsg.Port = channel.Port;
						cmsg.Type = 0; // TODO
						cmsgs.Add(cmsg);
					}
					smsg.Channels.AddRange(cmsgs);
					outData.Add(smsg);
				}
				return outData;
			}
		}

		public void PrintChannels()
		{
			Serilog.Log.Debug("Printing active channels:");
			lock (_servers)
			{
				foreach (Server server in _servers)
				{
					foreach (Channel channel in server.Channels)
					{
						channel.Print();
					}
				}
				
			}
		}

		public byte[]? GetSerializedServerData()
		{
			if(_servers.Count > 0) 
			{ 
				byte[] data = new byte[_servers.Count*2];
				int i = 0;
				foreach(Server server in _servers)
				{
					data[i] = (byte)server.Id;
					i += 2;
				}
				return data;
			}
			return null;
		}

	}
}
