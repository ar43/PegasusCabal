using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.DB
{
	internal class SubpassManager
	{
		private NpgsqlDataSource _dataSource;

		public SubpassManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public async Task<(string, DateTime?)> GetSubPasswordData(int accountId)
		{
			using var conn = await _dataSource.OpenConnectionAsync();
			string subpass = "";
			DateTime? dateTime = null;

			await using (var cmd = new NpgsqlCommand("SELECT subpass, last_verification FROM main.subpass WHERE account_id=@p", conn))
			{
				cmd.Parameters.AddWithValue("p", accountId);
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						subpass = reader.GetString(0);
						if(!reader.IsDBNull(1))
						{
							dateTime = reader.GetDateTime(1);
						}
						else
						{
							dateTime = null;
						}
					}
				}
			}

			return (subpass,dateTime);
		}

		public async Task<bool> SetSubpass(int accountId, string subpass)
		{
			using var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("INSERT INTO main.subpass VALUES (@a, @b)", conn))
			{
				try
				{
					cmd.Parameters.AddWithValue("a", accountId);
					cmd.Parameters.AddWithValue("b", subpass);
					await cmd.ExecuteNonQueryAsync();
					return true;
				}
				catch
				{
					return false;
				}
			}
		}

		public async Task<bool> SetSubPasswordVerificationDate(int accountId, DateTime date)
		{
			using var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.subpass SET last_verification = @a WHERE account_id = @b", conn))
			{
				try
				{
					cmd.Parameters.AddWithValue("b", accountId);
					cmd.Parameters.AddWithValue("a", date);
					await cmd.ExecuteNonQueryAsync();
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
	}
}
