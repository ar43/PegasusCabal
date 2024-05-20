using Google.Protobuf;
using Npgsql;
using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.DbSyncData;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorldServer.DB
{
	internal class CharacterManager
	{
		private NpgsqlDataSource _dataSource;

		public CharacterManager(NpgsqlDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		private static T? ToProtoObject<T>(byte[] buf, int len) where T : IMessage<T>, new()
		{
			if (buf == null)
				return default(T);

			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(buf, 0, len);
				ms.Seek(0, SeekOrigin.Begin);

				MessageParser<T> parser = new MessageParser<T>(() => new T());
				return parser.ParseFrom(ms);
			}
		}

		public async Task<int> SyncEquipment(int charId, EquipmentData equipmentData)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.characters SET eq_data = @a WHERE char_id = @b", conn))
			{
				try
				{
					cmd.Parameters.AddWithValue("b", charId);
					cmd.Parameters.AddWithValue("a", equipmentData.ToByteArray());
					var result = await cmd.ExecuteNonQueryAsync();
					return result;
				}
				catch
				{
					throw;
				}
			}
		}

		public async Task<int> SyncInventory(int charId, InventoryData inventoryData)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.characters SET inv_data = @a WHERE char_id = @b", conn))
			{
				try
				{
					cmd.Parameters.AddWithValue("b", charId);
					cmd.Parameters.AddWithValue("a", inventoryData.ToByteArray());
					var result = await cmd.ExecuteNonQueryAsync();
					return result;
				}
				catch
				{
					throw;
				}
			}
		}

		public async Task<int> SyncSkills(int charId, SkillData skillData)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.characters SET skill_data = @a WHERE char_id = @b", conn))
			{
				try
				{
					cmd.Parameters.AddWithValue("b", charId);
					cmd.Parameters.AddWithValue("a", skillData.ToByteArray());
					var result = await cmd.ExecuteNonQueryAsync();
					return result;
				}
				catch
				{
					throw;
				}
			}
		}

		public async Task<int> SyncLinks(int charId, QuickSlotData linksData)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.characters SET quickslot_data = @a WHERE char_id = @b", conn))
			{
				try
				{
					cmd.Parameters.AddWithValue("b", charId);
					cmd.Parameters.AddWithValue("a", linksData.ToByteArray());
					var result = await cmd.ExecuteNonQueryAsync();
					return result;
				}
				catch
				{
					throw;
				}
			}
		}

		public async Task<int> SyncLocation(int charId, DbSyncLocation locationData)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.characters SET x = @b, y = @c, world_id = @d  WHERE char_id = @a", conn))
			{
				try
				{
					cmd.Parameters.AddWithValue("a", charId);
					cmd.Parameters.AddWithValue("b", locationData.X);
					cmd.Parameters.AddWithValue("c", locationData.Y);
					cmd.Parameters.AddWithValue("d", locationData.WorldId);
					var result = await cmd.ExecuteNonQueryAsync();
					return result;
				}
				catch
				{
					throw;
				}
			}
		}

		public async Task<int> SyncStats(int charId, DbSyncStats statsData)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.characters SET level = @b, exp = @c, str = @d, dex = @e, int = @int, pnt = @pnt, rank = @rank  WHERE char_id = @a", conn))
			{
				try
				{
					//todo axp
					cmd.Parameters.AddWithValue("a", charId);
					cmd.Parameters.AddWithValue("b", statsData.Level);
					cmd.Parameters.AddWithValue("c", statsData.Exp);
					cmd.Parameters.AddWithValue("d", statsData.Str);
					cmd.Parameters.AddWithValue("e", statsData.Dex);
					cmd.Parameters.AddWithValue("int", statsData.Int);
					cmd.Parameters.AddWithValue("pnt", statsData.Pnt);
					cmd.Parameters.AddWithValue("rank", statsData.Rank);
					var result = await cmd.ExecuteNonQueryAsync();
					return result;
				}
				catch
				{
					throw;
				}
			}
		}

		public async Task<int> SyncStatus(int charId, DbSyncStatus statusData)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("UPDATE main.characters SET hp = @hp, mp = @mp, sp = @sp, max_hp = @max_hp, max_mp = @max_mp, max_sp = @max_sp WHERE char_id = @a", conn))
			{
				try
				{
					//todo axp
					cmd.Parameters.AddWithValue("a", charId);
					cmd.Parameters.AddWithValue("hp", statusData.Hp);
					cmd.Parameters.AddWithValue("mp", statusData.Mp);
					cmd.Parameters.AddWithValue("sp", statusData.Sp);
					cmd.Parameters.AddWithValue("max_hp", statusData.MaxHp);
					cmd.Parameters.AddWithValue("max_sp", statusData.MaxSp);
					cmd.Parameters.AddWithValue("max_mp", statusData.MaxMp);
					var result = await cmd.ExecuteNonQueryAsync();
					return result;
				}
				catch
				{
					throw;
				}
			}
		}

		public async Task<(Character?, int)> GetCharacter(UInt32 charId)
		{
			var conn = await _dataSource.OpenConnectionAsync();

			await using (var cmd = new NpgsqlCommand("SELECT * FROM main.characters WHERE char_id=@p", conn))
			{
				cmd.Parameters.AddWithValue("p", (int)charId);
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					var idLabel = reader.GetOrdinal("char_id");
					var alz = reader.GetOrdinal("alz");
					var creationDate = reader.GetOrdinal("creation_date");
					var equipment = reader.GetOrdinal("eq_data");
					var level = reader.GetOrdinal("level");
					var name = reader.GetOrdinal("name");
					var rank = reader.GetOrdinal("rank");
					var style = reader.GetOrdinal("style");
					var worldId = reader.GetOrdinal("world_id");
					var x = reader.GetOrdinal("x");
					var y = reader.GetOrdinal("y");
					var exp = reader.GetOrdinal("exp");
					var str = reader.GetOrdinal("str");
					var dex = reader.GetOrdinal("dex");
					var INT = reader.GetOrdinal("int");
					var pnt = reader.GetOrdinal("pnt");
					var hp = reader.GetOrdinal("hp");
					var mp = reader.GetOrdinal("mp");
					var sp = reader.GetOrdinal("sp");
					var maxHp = reader.GetOrdinal("max_hp");
					var maxMp = reader.GetOrdinal("max_mp");
					var maxSp = reader.GetOrdinal("max_sp");
					var swdPnt = reader.GetOrdinal("swd_pnt");
					var magPnt = reader.GetOrdinal("mag_pnt");
					var rankExp = reader.GetOrdinal("rank_exp");
					var flags = reader.GetOrdinal("flags");
					var invData = reader.GetOrdinal("inv_data");
					var skillData = reader.GetOrdinal("skill_data");
					var quickslotData = reader.GetOrdinal("quickslot_data");

					if (await reader.ReadAsync())
					{
						byte[] dbDataBytes = new byte[1024];

						var dataLen = reader.GetBytes(equipment, 0, dbDataBytes, 0, dbDataBytes.Length);

						if (dataLen == dbDataBytes.Length)
						{
							throw new Exception("increase dbDataBytes buffer");
						}

						var eqProtobuf = ToProtoObject<EquipmentData>(dbDataBytes, (int)dataLen);
						Equipment cEquipment = new Equipment();

						foreach(var eq in eqProtobuf.EquipmentData_)
						{
							Item item = new Item(eq.Value.Kind, eq.Value.Option);
							cEquipment.List[eq.Key] = item;
						}

						Array.Clear(dbDataBytes);
						dataLen = reader.GetBytes(invData, 0, dbDataBytes, 0, dbDataBytes.Length);
						if (dataLen == dbDataBytes.Length)
						{
							throw new Exception("increase dbDataBytes buffer");
						}
						var invProtobuf = ToProtoObject<InventoryData>(dbDataBytes, (int)dataLen);
						Inventory cInventory = new Inventory();
						foreach(var inv in invProtobuf.InventoryData_)
						{
							cInventory.Items.Add((UInt16)inv.Key, new Item(inv.Value.Kind, inv.Value.Option));
						}

						cInventory.Alz = (UInt64)reader.GetInt64(alz);

						Array.Clear(dbDataBytes);
						dataLen = reader.GetBytes(skillData, 0, dbDataBytes, 0, dbDataBytes.Length);
						if (dataLen == dbDataBytes.Length)
						{
							throw new Exception("increase dbDataBytes buffer");
						}
						var skillsProtobuf = ToProtoObject<SkillData>(dbDataBytes, (int)dataLen);
						Skills cSkills = new Skills();
						foreach (var skill in skillsProtobuf.SkillData_)
						{
							cSkills.LearnedSkills.Add((UInt16)skill.Key, new Skill((UInt16)skill.Value.Id, (Byte)skill.Value.Level));
						}

						Array.Clear(dbDataBytes);
						dataLen = reader.GetBytes(quickslotData, 0, dbDataBytes, 0, dbDataBytes.Length);
						if (dataLen == dbDataBytes.Length)
						{
							throw new Exception("increase dbDataBytes buffer");
						}
						var linksProtobuf = ToProtoObject<QuickSlotData>(dbDataBytes, (int)dataLen);
						QuickSlotBar cQuickSlotBar = new QuickSlotBar();
						foreach (var link in linksProtobuf.QuickSlotData_)
						{
							cQuickSlotBar.Links.Add((UInt16)link.Key, new SkillLink((UInt16)link.Value.Id));
						}
						var wid = reader.GetInt32(worldId);
						Character character = new Character(
							new Style((UInt32)reader.GetInt32(style)),
							reader.GetString(name),
							cEquipment,
							cInventory,
							cSkills,
							cQuickSlotBar,
							new Location((UInt16)reader.GetInt32(x), (UInt16)reader.GetInt32(y)),
							new Stats((UInt32)reader.GetInt32(level), (UInt32)reader.GetInt32(exp), (UInt32)reader.GetInt32(str), (UInt32)reader.GetInt32(dex), (UInt32)reader.GetInt32(INT), (UInt32)reader.GetInt32(pnt), (UInt32)reader.GetInt32(rank)),
							new Status((UInt32)reader.GetInt32(hp), (UInt32)reader.GetInt32(maxHp), (UInt32)reader.GetInt32(mp), (UInt32)reader.GetInt32(maxMp), (UInt32)reader.GetInt32(sp), (UInt32)reader.GetInt32(maxSp)),
							reader.GetInt32(idLabel)
						);
						return (character, wid);
					}
				}
			}
			
			return (null, 0);
		}
	}

}
