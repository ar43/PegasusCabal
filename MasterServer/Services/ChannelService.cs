using Google.Protobuf;
using Grpc.Core;
using MasterServer.Channel;
using MasterServer.DB;
using Shared.Protos;

namespace MasterServer.Services
{
	public class ChannelService : ChannelServiceAbc.ChannelServiceAbcBase
	{
		private readonly ChannelManager _channelManager;

		public ChannelService(ChannelManager channelManager)
		{
			_channelManager = channelManager;
		}

		public override Task<ServerStateReply> GetServerState(ServerStateRequest request, ServerCallContext context)
		{
			List<ServerMsg> msg = _channelManager.GetServerMsg();
			return Task.FromResult(new ServerStateReply
			{
				ServerCount = (UInt32)msg.Count,
				Servers = {msg}
			});
		}
	}
}
