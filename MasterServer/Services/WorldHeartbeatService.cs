using Grpc.Core;
using MasterServer.Channel;
using Shared.Protos.World;

namespace MasterServer.Services
{
    public class WorldHeartbeatService : WorldHeartbeat.WorldHeartbeatBase
	{
		private readonly ChannelManager _channelManager;
		public WorldHeartbeatService(ChannelManager channelManager)
		{
			_channelManager = channelManager;
		}

		public override Task<WorldHeartbeatReply> Send(WorldHeartbeatRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called Worldheart.Send");
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
