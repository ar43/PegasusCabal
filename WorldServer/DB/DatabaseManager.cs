using Npgsql;

namespace WorldServer.DB
{
	internal class DatabaseManager
	{
		public NpgsqlDataSource DataSourceWorld { private set; get; }

		public SubpassManager SubpassManager { private set; get; }

		public DatabaseManager()
		{
			var cfg = ServerConfig.Get();
			var dataSourceBuilderWorld = new NpgsqlDataSourceBuilder(cfg.DatabaseSettings.ConnString);
			DataSourceWorld = dataSourceBuilderWorld.Build();
			SubpassManager = new(DataSourceWorld);
		}
	}
}
