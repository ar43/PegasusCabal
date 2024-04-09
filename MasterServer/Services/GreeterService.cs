using Grpc.Core;
using Shared.Protos;


namespace MasterServer.Services
{
	public class GreeterService : Greeter.GreeterBase
	{
		public GreeterService()
		{
		}

		public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			Serilog.Log.Information("yo");
			return Task.FromResult(new HelloReply
			{
				Message = "Hello " + request.Name
			});
		}
	}
}
