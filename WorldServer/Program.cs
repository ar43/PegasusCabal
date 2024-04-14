using Serilog;

namespace WorldServer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using var log = new LoggerConfiguration().WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose).MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose).CreateLogger();

			Log.Logger = log;
			Log.Information("Starting Pegasus WorldServer...");

			Server server = new();
			server.Run();

		}
	}
}
