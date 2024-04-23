using Grpc.Core;
using LibPegasus.JSON;
using MasterServer.Channel;
using MasterServer.DB;
using Shared.Protos;
using System.Text.Json;

namespace MasterServer.Services
{
	public class CharacterMasterService : CharacterMaster.CharacterMasterBase
	{

		private readonly DatabaseManager _databaseManager;

		public CharacterMasterService(DatabaseManager databaseManager)
		{
			_databaseManager = databaseManager;
		}

		public override Task<CreateCharacterReply> CreateCharacter(CreateCharacterRequest request, ServerCallContext context)
		{
			var jsonString = File.ReadAllText("..\\LibPegasus\\Data\\char_init.json");
			var json = JsonSerializer.Deserialize<CharInitRoot>(jsonString);
			var result = _databaseManager.CharacterManager.CreateCharacter(request, json.CharInitData[request.Class - 1]);
			return Task.FromResult(new CreateCharacterReply
			{
				Result = (UInt32)result.Result.Item2,
				CharId = (UInt32)result.Result.Item1
			});
		}
	}
}
