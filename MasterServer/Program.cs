using MasterServer.Channel;
using MasterServer.Chat;
using MasterServer.DB;
using MasterServer.Services;
using MasterServer.Sync;
using Serilog;

namespace MasterServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using var log = new LoggerConfiguration().WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information).MinimumLevel.Is(Serilog.Events.LogEventLevel.Information).CreateLogger();

			Log.Logger = log;
			Log.Information("Starting Pegasus MasterServer...");

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Configuration.AddJsonFile("dbsettings.json", optional: false, reloadOnChange: true);
			builder.Configuration.AddJsonFile("chatsettings.json", optional: false, reloadOnChange: true);
			builder.Host.UseSerilog();
			builder.Services.AddGrpc();
			builder.Services.AddScoped<DatabaseManager>();
			builder.Services.AddSingleton<ChannelManager>();
			builder.Services.AddSingleton<SyncManager>();
			builder.Services.AddHostedService<TimedChannelService>();
			builder.Services.AddSingleton<ChatServer>();
			//builder.Services.AddHostedService<ChatServerService>();

			var app = builder.Build();

			app.MapGrpcService<ChannelMasterService>();
			app.MapGrpcService<AuthMasterService>();
			app.MapGrpcService<CharacterMasterService>();
			app.MapGrpcService<ChatMasterService>();
			app.MapGet("/", () => "PegasusCabal MasterServer");

			app.Run();
		}
	}
}