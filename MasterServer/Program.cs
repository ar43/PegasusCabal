using MasterServer.Channel;
using MasterServer.DB;
using MasterServer.Services;
using Serilog;

namespace MasterServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using var log = new LoggerConfiguration().WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug).MinimumLevel.Is(Serilog.Events.LogEventLevel.Debug).CreateLogger();

			Log.Logger = log;
			Log.Information("Starting Pegasus MasterServer...");

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Configuration.AddJsonFile("dbsettings.json", optional: false, reloadOnChange: true);
			builder.Host.UseSerilog();
			builder.Services.AddGrpc();
			builder.Services.AddScoped<DatabaseManager>();
			builder.Services.AddSingleton<ChannelManager>();
			builder.Services.AddHostedService<TimedChannelService>();

			var app = builder.Build();

			app.MapGrpcService<ChannelMasterService>();
			app.MapGrpcService<AuthMasterService>();
			app.MapGet("/", () => "PegasusCabal MasterServer");

			app.Run();
		}
	}
}