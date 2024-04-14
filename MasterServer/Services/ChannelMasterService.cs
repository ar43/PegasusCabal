using Grpc.Core;
using MasterServer.Channel;
using Shared.Protos;

namespace MasterServer.Services
{
	public class ChannelMasterService : ChannelMaster.ChannelMasterBase
	{
		private readonly ChannelManager _channelManager;

		public ChannelMasterService(ChannelManager channelManager)
		{
			_channelManager = channelManager;
		}

		public override Task<ServerStateReply> GetServerState(ServerStateRequest request, ServerCallContext context)
		{
			List<ServerMsg> msg = _channelManager.GetServerMsg(request.IsLocalhost);
			return Task.FromResult(new ServerStateReply
			{
				ServerCount = (UInt32)msg.Count,
				Servers = { msg }
			});
		}

		public override Task<WorldHeartbeatReply> Heartbeat(WorldHeartbeatRequest request, ServerCallContext context)
		{
			var code = _channelManager.Heartbeat(request.ServerId, request.ChannelId, request.Ip, request.Port);
			return Task.FromResult(new WorldHeartbeatReply
			{
				InfoCode = (uint)code
			});
		}
	}

	public class TimedChannelService : BackgroundService
	{
		private int _executionCount;
		private readonly ChannelManager _channelManager;

		public TimedChannelService(ChannelManager channelManager)
		{
			_channelManager = channelManager;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Serilog.Log.Debug("Timed Channel Service running.");

			// When the timer should have no due-time, then do the work once now.
			CheckForExpiredChannels();

			using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));

			try
			{
				while (await timer.WaitForNextTickAsync(stoppingToken))
				{
					CheckForExpiredChannels();
				}
			}
			catch (OperationCanceledException)
			{
				Serilog.Log.Debug("Timed Channel Service is stopping.");
			}
		}

		private void CheckForExpiredChannels()
		{
			int count = Interlocked.Increment(ref _executionCount);

			_channelManager.RemoveExpiredChannels();
			_channelManager.PrintChannels();

			//Serilog.Log.Debug("Timed Hosted Service is working. Count: {Count}", count);
		}
	}
}
