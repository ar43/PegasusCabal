using BCrypt.Net;
using LibPegasus.Enums;
using Npgsql;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.DB
{
	public class AccountManager
	{
		private NpgsqlDataSource _dataSource;

		public AccountManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		private async Task<bool> AccountExists(string username)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM main.accounts WHERE username=@p)", conn))
			{
				cmd.Parameters.AddWithValue("p", username);
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					bool found = await reader.ReadAsync();
					if (found)
					{
						bool output = reader.GetBoolean(0);
						return output;
					}
					else
					{
						return false;
					}
				}
			}
		}

		private async Task<string> AccountVerify(string username, string password)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("SELECT password FROM main.accounts WHERE username=@p", conn))
			{
				cmd.Parameters.AddWithValue("p", username);
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					bool found = await reader.ReadAsync();
					if (found)
					{
						var output = reader.GetString(0);
						Debug.Assert(output != String.Empty);
						return output;
					}
					else
					{
						return String.Empty;
					}
				}
			}
		}

		private async Task<bool> RegisterAccount(string username, string password)
		{
			var hashPassword = Task.Factory.StartNew(() =>
			{
				string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
				return passwordHash;
			});

			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("INSERT INTO main.accounts VALUES (@u, @p)", conn))
			{
				cmd.Parameters.AddWithValue("u", username);
				cmd.Parameters.AddWithValue("p", hashPassword.Result);
				await cmd.ExecuteNonQueryAsync();
				return true;
			}
		}

		public async Task<InfoCodeLS> RequestRegister(string username, string password)
		{
			// TODO

			bool exists = await AccountExists(username);

			if (exists)
			{
				return InfoCodeLS.REGISTRATION_USEREXISTS;
			}
			else
			{
				bool accountRegistered = await RegisterAccount(username, password);

				if (accountRegistered)
				{
					Log.Information($"Registered account with username {username}");
					return InfoCodeLS.REGISTRATION_OK;
				}
				else
				{
					return InfoCodeLS.REGISTRATION_FAILED;
				}
			}
		}

		public async Task<bool> RequestLogin(string username, string password, long time, byte[] ip, string loginSecret)
		{
			var passwordHash = await AccountVerify(username, password);

			if (passwordHash == String.Empty)
			{
				return false;
			}
			else
			{
				var valid = Task.Factory.StartNew(() =>
				{
					var validation = BCrypt.Net.BCrypt.Verify(password, passwordHash);
					return validation;
				});

				if (valid.Result)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}


	}
}
