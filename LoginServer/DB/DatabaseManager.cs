using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibPegasus.DB;
using Npgsql;

namespace LibPegasus.DB
{
    internal class DatabaseManager
    {
		private NpgsqlDataSource _dataSource;
		internal AccountManager AccountManager { private set; get; }

        internal DatabaseManager(string connString)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
            _dataSource = dataSourceBuilder.Build();

			AccountManager = new(_dataSource);
        }
    }
}
