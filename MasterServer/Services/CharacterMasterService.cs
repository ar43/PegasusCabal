using Grpc.Core;
using LibPegasus.JSON;
using MasterServer.Channel;
using MasterServer.DB;
using MasterServer.Sync;
using Shared.Protos;
using System.Text.Json;

namespace MasterServer.Services
{
	public class CharacterMasterService : CharacterMaster.CharacterMasterBase
	{

		private readonly DatabaseManager _databaseManager;
		private readonly SyncManager _syncManager;

		public CharacterMasterService(DatabaseManager databaseManager, SyncManager syncManager)
		{
			_databaseManager = databaseManager;
			_syncManager = syncManager;
		}

		public override Task<CreateCharacterReply> CreateCharacter(CreateCharacterRequest request, ServerCallContext context)
		{
			var jsonString = File.ReadAllText("..\\LibPegasus\\Data\\char_init_dev.json");
			var json = JsonSerializer.Deserialize<CharInitRoot>(jsonString);
			var result = _databaseManager.CharacterManager.CreateCharacter(request, json.CharInitData[(request.Style & 7) - 1]);
			return Task.FromResult(new CreateCharacterReply
			{
				Result = (UInt32)result.Result.Item2,
				CharId = (UInt32)result.Result.Item1
			});
		}

		public override Task<CharacterSyncStatusReply> CharacterSyncStatus(CharacterSyncStatusRequest request, ServerCallContext context)
		{
			//todo: serverId
			var result = _syncManager.IsSynced((Int32)request.CharId);
			return Task.FromResult(new CharacterSyncStatusReply
			{
				Status = Convert.ToUInt32(result)
			});
		}

		public override Task<SetCharacterSyncStatusReply> SetCharacterSyncStatus(SetCharacterSyncStatusRequest request, ServerCallContext context)
		{
			//todo: exception on resync
			_syncManager.SyncSection((Int32)request.CharId, (LibPegasus.Enums.SyncFlags)request.SyncFlags);
			return Task.FromResult(new SetCharacterSyncStatusReply
			{
				Status = 1
			});
		}

		public override Task<GetMyCharactersReply> GetMyCharacters(GetMyCharactersRequest request, ServerCallContext context)
		{
			var output = _databaseManager.CharacterManager.GetMyCharacters(request);
			//return output.Result;
			return Task.FromResult(new GetMyCharactersReply
			{
				Characters = { output.Result.Characters },
				CharacterOrder = output.Result.CharacterOrder,
				IsPinSet = output.Result.IsPinSet,
				LastCharId = output.Result.LastCharId
			});
		}
	}
}
