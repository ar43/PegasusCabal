using Grpc.Core;
using LibPegasus.Utils;
using MasterServer.Channel;
using MasterServer.Chat;
using MasterServer.DB;
using Shared.Protos;
using System.Net;

namespace MasterServer.Services
{
	public class ChatMasterService : ChatMaster.ChatMasterBase
	{
		private readonly ChatServer _chatServer;
		public ChatMasterService(ChatServer chatServer) 
		{
			_chatServer = chatServer;
		}

		public override Task<GetChatServerInfoReply> GetChatServerInfo(GetChatServerInfoRequest request, ServerCallContext context)
		{
			var isLocalhost = request.IsLocalhost;
			string ip;
			if(isLocalhost)
			{
				ip = "127.0.0.1";
			}
            else
            {
				ip = _chatServer.Ip.ToString();
			}
            return Task.FromResult(new GetChatServerInfoReply
			{
				Ip = ip,
				Port = _chatServer.Port,
			});
		}
	}

	public class ChatServerService : BackgroundService
	{
		private readonly ChatServer _chatServer;

		public ChatServerService(ChatServer chatServer)
		{
			_chatServer = chatServer;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			Serilog.Log.Debug("Attempting to run Chat Server...");

			await Task.Factory.StartNew(() => _chatServer.Run(), TaskCreationOptions.LongRunning);
		}
	}
}
