using LibPegasus.DB;
using Npgsql;

namespace MasterServer.DB
{
	public class DatabaseManager
	{
		private NpgsqlDataSource _dataSourceAuth;
		private NpgsqlDataSource _dataSourceWorld;

		public AccountManager AccountManager { private set; get; }

		public CharacterManager CharacterManager { private set; get; }

		public SessionManager SessionManager { private set; get; }

		//TODO: add other managers

		public DatabaseManager(IConfiguration configuration)
		{
			var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration["ConnStringAuth"]);
			_dataSourceAuth = dataSourceBuilder.Build();

			var dataSourceBuilderWorld = new NpgsqlDataSourceBuilder(configuration["ConnStringWorld"]);
			_dataSourceWorld = dataSourceBuilderWorld.Build();

			AccountManager = new(_dataSourceAuth);
			CharacterManager = new(_dataSourceWorld);
			SessionManager = new(_dataSourceWorld);
		}
	}
}
