using Serilog;

namespace LibPegasus
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using var log = new LoggerConfiguration().WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose).MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose).CreateLogger();
			
			Log.Logger = log;
			Log.Information("Starting Pegasus LoginServer...");

			Server server = new();
			server.Run();

		}
	}
}
