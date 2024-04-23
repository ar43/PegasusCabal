using Npgsql;

namespace MasterServer.DB
{
	public class DatabaseManager
	{
		private NpgsqlDataSource _dataSourceAuth;
		private NpgsqlDataSource _dataSourceWorld;

		public AccountManager AccountManager { private set; get; }

		public CharacterManager CharacterManager { private set; get; }

		public SessionManager WorldSessionManager { private set; get; }

		public SessionManager LoginSessionManager { private set; get; }

		//TODO: add other managers

		public DatabaseManager(IConfiguration configuration)
		{
			var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration["ConnStringAuth"]);
			_dataSourceAuth = dataSourceBuilder.Build();

			var dataSourceBuilderWorld = new NpgsqlDataSourceBuilder(configuration["ConnStringWorld"]);
			_dataSourceWorld = dataSourceBuilderWorld.Build();

			AccountManager = new(_dataSourceAuth);
			LoginSessionManager = new(_dataSourceAuth);
			CharacterManager = new(_dataSourceWorld);
			WorldSessionManager = new(_dataSourceWorld);
		}
	}
}
