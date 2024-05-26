using Google.Protobuf;
using Grpc.Core;
using LibPegasus.Enums;
using MasterServer.Channel;
using MasterServer.DB;
using Shared.Protos;

namespace MasterServer.Services
{
	public class AuthMasterService : AuthMaster.AuthMasterBase
	{
		private readonly DatabaseManager _databaseManager;
		private readonly ChannelManager _channelManager;

		public AuthMasterService(DatabaseManager databaseManager, ChannelManager channelManager)
		{
			_databaseManager = databaseManager;
			_channelManager = channelManager;
		}

		public override Task<RegisterAccountReply> Register(RegisterAccountRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called Register");
			var code = _databaseManager.AccountManager.RequestRegister(request.Username, request.Password);
			Serilog.Log.Information("Registration return code: " + code.Result);
			return Task.FromResult(new RegisterAccountReply
			{
				InfoCode = (uint)code.Result
			});
		}

		public override Task<SessionReply> CreateSession(SessionRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called CreateSession");
			var code = _databaseManager.WorldSessionManager.Create(request.AuthKey, (UInt16)request.UserId, (byte)request.ChannelId, (byte)request.ServerId, request.AccountId);
			//Serilog.Log.Information("Registration return code: " + code.Result);
			return Task.FromResult(new SessionReply
			{
				//InfoCode = (uint)code.Result
				Result = (uint)code.Result
			});
		}

		public override Task<SessionReply> CreateLoginSession(SessionRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called CreateLoginSession");
			var code = _databaseManager.LoginSessionManager.Create(request.AuthKey, (UInt16)request.UserId, (byte)request.ChannelId, (byte)request.ServerId, request.AccountId);
			//Serilog.Log.Information("Registration return code: " + code.Result);
			return Task.FromResult(new SessionReply
			{
				//InfoCode = (uint)code.Result
				Result = (uint)code.Result
			});
		}

		public override Task<LoginAccountReply> Login(LoginAccountRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("Called Login");
			AuthResult status = AuthResult.None;
			var accountId = _databaseManager.AccountManager.RequestLogin(request.Username, request.Password);
			//Serilog.Log.Information("Login return code: " + success.Result);

			var serverData = _channelManager.GetSerializedServerData();
			var serverCount = 0;

			//TODO: send bad result if acc is already logged in
			if (accountId.Result > 0)
			{
				status = AuthResult.Normal;

				var charCountData = _databaseManager.CharacterManager.GetCharacterCount((int)accountId.Result);
				if (serverData != null)
				{
					serverCount = serverData.Length / 2;
					for (int i = 0; i < serverData.Length; i += 2)
					{
						charCountData.Result.TryGetValue(serverData[i], out int charCount);
						serverData[i + 1] = (Byte)charCount;
					}
				}
			}
			else
			{
				status = AuthResult.Incorrect;
			}

			return Task.FromResult(new LoginAccountReply
			{
				Status = (uint)status,
				AccountId = accountId.Result,
				ServerCount = (uint)serverCount,
				SubPassSet = false, //TODO
				CharData = ByteString.CopyFrom(serverData),
				PremServId = 0,
				PremServExpired = 0,
				Language = 0,
				AuthKey = "46385170829535025841897130667207"
			});
		}
	}
}
