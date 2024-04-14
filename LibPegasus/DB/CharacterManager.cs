using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.DB
{
	public class CharacterManager
	{
		private NpgsqlDataSource _dataSource;

		public CharacterManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public async Task<Dictionary<int,int>> GetCharacterCount(int accountId)
		{
			var conn = await _dataSource.OpenConnectionAsync();
			var dict = new Dictionary<int,int>();

			await using (var cmd = new NpgsqlCommand("SELECT server_id FROM main.characters WHERE account_id=@p", conn))
			{
				cmd.Parameters.AddWithValue("p", accountId);
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						var serverId = reader.GetInt32(0);
						dict.TryGetValue(serverId, out Int32 currentCount);
						dict[serverId] = currentCount + 1;
					}
				}
			}

			return dict;
		}
	}
}
