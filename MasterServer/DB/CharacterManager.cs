using Google.Protobuf;
using LibPegasus.Enums;
using LibPegasus.JSON;
using Npgsql;
using Shared.Protos;
using System;
using System.Runtime.Serialization.Json;
using static Shared.Protos.SkillData.Types;

namespace MasterServer.DB
{
	public class CharacterManager
	{
		private NpgsqlDataSource _dataSource;

		public CharacterManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public async Task<Dictionary<int, int>> GetCharacterCount(int accountId)
		{
			var conn = await _dataSource.OpenConnectionAsync();
			var dict = new Dictionary<int, int>();

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

		private static InventoryData JsonToProtobuf(Dictionary<uint, InventoryDataItem> invData)
		{
			var invSerial = new InventoryData();
			foreach(var item in invData)
			{
				invSerial.InventoryData_.Add(item.Key, new InventoryData.Types.InventoryDataItem { Kind = item.Value.Kind, Option = item.Value.Option});
			}
			return invSerial;
		}
		private static EquipmentData JsonToProtobuf(Dictionary<uint, EquipmentDataItem> invData)
		{
			var invSerial = new EquipmentData();
			foreach (var item in invData)
			{
				invSerial.EquipmentData_.Add(item.Key, new EquipmentData.Types.EquipmentDataItem { Kind = item.Value.Kind, Option = 0 });
			}
			return invSerial;
		}

		private static SkillData JsonToProtobuf(Dictionary<ushort, SkillDataEntry> invData)
		{
			var invSerial = new SkillData();
			foreach (var item in invData)
			{
				invSerial.SkillData_.Add(item.Key, new SkillData.Types.SkillDataItem { Id = item.Value.Id, Level = item.Value.Level });
			}
			return invSerial;
		}

		private static QuickSlotData JsonToProtobuf(Dictionary<ushort, QuickSlotDataEntry> invData)
		{
			var invSerial = new QuickSlotData();
			foreach (var item in invData)
			{
				invSerial.QuickSlotData_.Add(item.Key, new QuickSlotData.Types.QuickSlotDataItem { Id = item.Value.Id});
			}
			return invSerial;
		}

		public async Task<(int, CharCreateResult)> CreateCharacter(CreateCharacterRequest createCharacterRequest, CharInitData charInitData)
		{
			var conn = await _dataSource.OpenConnectionAsync();
			var charId = createCharacterRequest.AccountId * 8 + createCharacterRequest.Slot;
			var styleSerial = new StyleSerial { Aura = createCharacterRequest.Aura, BattleStyle = createCharacterRequest.Class, Face = createCharacterRequest.Face, Gender = Convert.ToBoolean(createCharacterRequest.Gender), HairColor = createCharacterRequest.HairColor, HairStyle = createCharacterRequest.HairStyle, Rank = createCharacterRequest.Rank, ShowHelmet = Convert.ToBoolean(createCharacterRequest.ShowHelmet) };
			var invSerial = JsonToProtobuf(charInitData.InventoryData);
			var eqSerial = JsonToProtobuf(charInitData.EquipmentData);
			var skillSerial = JsonToProtobuf(charInitData.SkillData);
			var quickslotSerial = JsonToProtobuf(charInitData.QuickSlotData);

			if(charInitData.ClassType != createCharacterRequest.Class)
			{
				return (0, CharCreateResult.DATABRK);
			}

			await using (var cmd = new NpgsqlCommand("INSERT INTO main.characters VALUES (@c1, @c2, @c3, @c4, @c5, @c6, @c7, @c8, @c9, @c10, @c11, @c12, @c13, @c14, @c15, @c16, @c17, @c18, @c19, @c20, @c21, @c22, @c23, @c24, @c25, @c26, @c27, @c28, @c29, @c30) RETURNING char_id", conn))
			{
				cmd.Parameters.AddWithValue("c1", (int)charId);
				cmd.Parameters.AddWithValue("c2", (int)createCharacterRequest.AccountId);
				cmd.Parameters.AddWithValue("c3", createCharacterRequest.Name);
				cmd.Parameters.AddWithValue("c4", (int)createCharacterRequest.ServerId);
				cmd.Parameters.AddWithValue("c5", styleSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c6", (int)charInitData.LEV);
				cmd.Parameters.AddWithValue("c7", (long)charInitData.Exp);
				cmd.Parameters.AddWithValue("c8", (int)charInitData.STR);
				cmd.Parameters.AddWithValue("c9", (int)charInitData.DEX);
				cmd.Parameters.AddWithValue("c10", (int)charInitData.INT);
				cmd.Parameters.AddWithValue("c11", (int)charInitData.PNT);
				cmd.Parameters.AddWithValue("c12", (int)charInitData.Rank);
				cmd.Parameters.AddWithValue("c13", (long)charInitData.Alz);
				cmd.Parameters.AddWithValue("c14", charInitData.WorldIdx);
				cmd.Parameters.AddWithValue("c15", (int)charInitData.Position.X);
				cmd.Parameters.AddWithValue("c16", (int)charInitData.Position.Y);
				cmd.Parameters.AddWithValue("c17", charInitData.HP);
				cmd.Parameters.AddWithValue("c18", charInitData.MP);
				cmd.Parameters.AddWithValue("c19", charInitData.SP);
				cmd.Parameters.AddWithValue("c20", charInitData.SwdPNT);
				cmd.Parameters.AddWithValue("c21", charInitData.MagPNT);
				cmd.Parameters.AddWithValue("c22", charInitData.RankEXP);
				cmd.Parameters.AddWithValue("c23", charInitData.Flags);
				cmd.Parameters.AddWithValue("c24", charInitData.WarpBField);
				cmd.Parameters.AddWithValue("c25", charInitData.MapsBField);
				cmd.Parameters.AddWithValue("c26", invSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c27", eqSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c28", skillSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c29", quickslotSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c30", DateTime.UtcNow);
				var output = await cmd.ExecuteScalarAsync();
				if(output != null)
				{
					return ((int)output,CharCreateResult.SUCCESS);
				}
				else
				{
					return (0, CharCreateResult.DBERROR);
				}
			}
		}
	}
}
