using LibPegasus.DB;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.DB
{
	internal class DatabaseManager
	{
		public NpgsqlDataSource DataSourceWorld { private set; get; }

		public CharacterManager CharacterManager { private set; get; }

		public SessionManager SessionManager { private set; get; }

		//TODO: add other managers

		public DatabaseManager()
		{
			var cfg = ServerConfig.Get();
			var dataSourceBuilderWorld = new NpgsqlDataSourceBuilder(cfg.DatabaseSettings.ConnString);
			DataSourceWorld = dataSourceBuilderWorld.Build();

			CharacterManager = new(DataSourceWorld);
			SessionManager = new(DataSourceWorld);
		}
	}
}
