using LibPegasus.DB;
using Npgsql;

namespace MasterServer.DB
{
	public class DatabaseManager
	{
		private NpgsqlDataSource _dataSource;

		public AccountManager AccountManager { private set; get; }

		//TODO: add other managers

		public DatabaseManager(IConfiguration configuration)
		{
			var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration["ConnString"]);
			_dataSource = dataSourceBuilder.Build();

			AccountManager = new(_dataSource);
		}
	}
}
