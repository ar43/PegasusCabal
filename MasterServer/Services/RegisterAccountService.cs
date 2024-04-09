using Grpc.Core;
using MasterServer.DB;
using Shared.Protos;

namespace MasterServer.Services
{
	public class RegisterAccountService : RegisterAccount.RegisterAccountBase
	{
		private readonly DatabaseManager _databaseManager;

		public RegisterAccountService(DatabaseManager databaseManager)
		{
			_databaseManager = databaseManager;
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
	}
}
