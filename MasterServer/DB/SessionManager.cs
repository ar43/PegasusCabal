using LibPegasus.Enums;
using Npgsql;

namespace MasterServer.DB
{
	public class SessionManager
	{
		private NpgsqlDataSource _dataSource;

		public SessionManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}
		public async Task<SessionResult> Create(UInt32 authKey, UInt16 userId, Byte channelId, Byte serverId, UInt32 accountId)
		{
			var conn = await _dataSource.OpenConnectionAsync();
			var result = SessionResult.OK;
			await using (var cmd = new NpgsqlCommand("DELETE FROM main.sessions WHERE account_id=@p RETURNING *", conn))
			{
				cmd.Parameters.AddWithValue("p", (int)accountId);
				var output = await cmd.ExecuteScalarAsync();
				if (output != null)
				{
					var outputInt = (int)output;
					if (outputInt != 0)
					{
						result = SessionResult.REPLACED;
					}
				}
			}

			await using (var cmd = new NpgsqlCommand("INSERT INTO main.sessions VALUES (@a, @b, @c, @d, @e)", conn))
			{
				cmd.Parameters.AddWithValue("a", (int)authKey);
				cmd.Parameters.AddWithValue("b", (int)userId);
				cmd.Parameters.AddWithValue("c", (int)channelId);
				cmd.Parameters.AddWithValue("d", (int)serverId);
				cmd.Parameters.AddWithValue("e", (int)accountId);
				await cmd.ExecuteNonQueryAsync();
			}
			return result;
		}
	}
}
