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
			using var conn = await _dataSource.OpenConnectionAsync();
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

		private static InventoryData JsonToProtobuf(Dictionary<uint, InventoryDataJSONItem> invData)
		{
			var invSerial = new InventoryData();
			foreach(var item in invData)
			{
				invSerial.InventoryData_.Add(item.Key, new ItemData { Kind = item.Value.Kind, Option = item.Value.Option, Duration = 0, Serial = 0});
			}
			return invSerial;
		}
		private static EquipmentData JsonToProtobuf(Dictionary<uint, EquipmentDataJSONItem> invData)
		{
			var invSerial = new EquipmentData();
			foreach (var item in invData)
			{
				invSerial.EquipmentData_.Add(item.Key, new ItemData { Kind = item.Value.Kind, Option = 0, Duration = 0, Serial = 0 });
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

		public async Task<GetMyCharactersReply> GetMyCharacters(GetMyCharactersRequest request)
		{
			using var conn = await _dataSource.OpenConnectionAsync();
			
			GetMyCharactersReply reply = new GetMyCharactersReply();
			
			await using (var cmd = new NpgsqlCommand("SELECT * FROM main.characters WHERE account_id=@p AND server_id=@g", conn))
			{
				cmd.Parameters.AddWithValue("p", (int)request.AccountId);
				cmd.Parameters.AddWithValue("g", (int)request.ServerId);
				await using (var reader = await cmd.ExecuteReaderAsync())
				{
					var alz = reader.GetOrdinal("alz");
					var charId = reader.GetOrdinal("char_id");
					var creationDate = reader.GetOrdinal("creation_date");
					var equipment = reader.GetOrdinal("eq_data");
					var level = reader.GetOrdinal("level");
					var name = reader.GetOrdinal("name");
					var rank = reader.GetOrdinal("rank");
					var style = reader.GetOrdinal("style");
					var worldId = reader.GetOrdinal("world_id");
					var x = reader.GetOrdinal("x");
					var y = reader.GetOrdinal("y");
					while (await reader.ReadAsync())
					{
						byte[] eqDataBytes = new byte[1024];

						var t = reader.GetBytes(equipment, 0, eqDataBytes, 0, eqDataBytes.Length);

						if(t == eqDataBytes.Length)
						{
							throw new Exception("increase eqDataBytes buffer");
						}

						var eqData = ToProtoObject<EquipmentData>(eqDataBytes, (int)t);

						UInt32[] equipmentData = new uint[20];
						for(int i = 0; i < 20; i++)
						{
							if (eqData != null && eqData.EquipmentData_.TryGetValue((uint)i, out var data))
							{
								equipmentData[i] = data.Kind;
							}
						}

						GetMyCharactersReplySingle character = new GetMyCharactersReplySingle
						{
							Alz = (UInt64)reader.GetInt64(alz),
							CharacterId = (UInt32)reader.GetInt32(charId),
							CreationDate = ((DateTimeOffset)reader.GetDateTime(creationDate)).ToUnixTimeSeconds(),
							Equipment = { equipmentData },
							Level = (UInt32)reader.GetInt32(level),
							Name = reader.GetString(name),
							Rank = (UInt32)reader.GetInt32(rank),
							Style = (UInt32)reader.GetInt32(style),
							U2 = 0,
							WorldId = (UInt32)reader.GetInt32(worldId),
							X = (UInt32)reader.GetInt32(x),
							Y = (UInt32)reader.GetInt32(y)
						};
						reply.Characters.Add(character);
					}
				}
			}
			reply.CharacterOrder = 0;
			reply.LastCharId = 0;
			reply.IsPinSet = false;
			return reply;
		}

		public async Task<(int, CharCreateResult)> CreateCharacter(CreateCharacterRequest createCharacterRequest, CharInitData charInitData)
		{
			using var conn = await _dataSource.OpenConnectionAsync();
			var charId = createCharacterRequest.AccountId * 8 + createCharacterRequest.Slot;
			var invSerial = JsonToProtobuf(charInitData.InventoryData);
			var eqSerial = JsonToProtobuf(charInitData.EquipmentData);
			var skillSerial = JsonToProtobuf(charInitData.SkillData);
			var quickslotSerial = JsonToProtobuf(charInitData.QuickSlotData);
			var time = DateTime.UtcNow;
			int defaultNation = 0;

			if((createCharacterRequest.Style & 7) != charInitData.ClassType)
			{
				return (0, CharCreateResult.DATABRK);
			}

			await using (var cmd = new NpgsqlCommand("INSERT INTO main.characters VALUES (@c1, @c2, @c3, @c4, @c5, @c6, @c7, @c8, @c9, @c10, @c11, @c12, @c13, @c14, @c15, @c16, @c17, @c18, @c19, @c20, @c21, @c22, @c23, @c24, @c25, @c26, @c27, @c28, @c29, @c30, @c31, @c32, @c33, @c34) RETURNING char_id", conn))
			{
				cmd.Parameters.AddWithValue("c1", (int)charId);
				cmd.Parameters.AddWithValue("c2", (int)createCharacterRequest.AccountId);
				cmd.Parameters.AddWithValue("c3", createCharacterRequest.Name);
				cmd.Parameters.AddWithValue("c4", (int)createCharacterRequest.ServerId);
				cmd.Parameters.AddWithValue("c5", (int)charInitData.LEV);
				cmd.Parameters.AddWithValue("c6", (long)charInitData.Exp);
				cmd.Parameters.AddWithValue("c7", (int)charInitData.STR);
				cmd.Parameters.AddWithValue("c8", (int)charInitData.DEX);
				cmd.Parameters.AddWithValue("c9", (int)charInitData.INT);
				cmd.Parameters.AddWithValue("c10", (int)charInitData.PNT);
				cmd.Parameters.AddWithValue("c11", (int)charInitData.Rank);
				cmd.Parameters.AddWithValue("c12", (long)charInitData.Alz);
				cmd.Parameters.AddWithValue("c13", charInitData.WorldIdx);
				cmd.Parameters.AddWithValue("c14", (int)charInitData.Position.X);
				cmd.Parameters.AddWithValue("c15", (int)charInitData.Position.Y);
				cmd.Parameters.AddWithValue("c16", charInitData.HP);
				cmd.Parameters.AddWithValue("c17", charInitData.MP);
				cmd.Parameters.AddWithValue("c18", charInitData.SP);
				cmd.Parameters.AddWithValue("c19", charInitData.SwdPNT);
				cmd.Parameters.AddWithValue("c20", charInitData.MagPNT);
				cmd.Parameters.AddWithValue("c21", charInitData.RankEXP);
				cmd.Parameters.AddWithValue("c22", charInitData.Flags);
				cmd.Parameters.AddWithValue("c23", charInitData.WarpBField);
				cmd.Parameters.AddWithValue("c24", charInitData.MapsBField);
				cmd.Parameters.AddWithValue("c25", invSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c26", eqSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c27", skillSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c28", quickslotSerial.ToByteArray());
				cmd.Parameters.AddWithValue("c29", time);
				cmd.Parameters.AddWithValue("c30", (int)createCharacterRequest.Style);
				cmd.Parameters.AddWithValue("c31", charInitData.SP);
				cmd.Parameters.AddWithValue("c32", charInitData.HP);
				cmd.Parameters.AddWithValue("c33", charInitData.MP);
				cmd.Parameters.AddWithValue("c34", defaultNation);
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
