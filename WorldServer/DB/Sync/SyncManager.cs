using Grpc.Net.Client;
using LibPegasus.Enums;
using Shared.Protos;
using System.Collections.Concurrent;
using WorldServer.Enums;
using WorldServer.Logic.CharData.DbSyncData;

namespace WorldServer.DB.Sync
{
	internal class SyncManager
	{
		ConcurrentQueue<DbSyncRequest>[] _requestQueue;
		GrpcChannel _masterRpcChannel;
		DatabaseManager _databaseManager;

		private volatile bool _running;
		private ConcurrentDictionary<int, SyncTimestamps> _syncTimestamps;

		public SyncManager(GrpcChannel masterRpcChannel, DatabaseManager databaseManager)
		{
			_requestQueue = new ConcurrentQueue<DbSyncRequest>[(Int32)DBSyncPriority.NUM_PRIORITIES];
			for (int i = 0; i < _requestQueue.Length; i++)
			{
				_requestQueue[i] = new();
			}
			_masterRpcChannel = masterRpcChannel;
			_databaseManager = databaseManager;
			_syncTimestamps = new ConcurrentDictionary<int, SyncTimestamps>();
			_running = true;

			Task.Factory.StartNew(() => Run(), TaskCreationOptions.LongRunning);
		}

		public void AddToQueue(DbSyncRequest request, DBSyncPriority prio)
		{
			_requestQueue[(Int32)prio].Enqueue(request);
		}

		private void Run()
		{
			while (_running || _requestQueue[(Int32)DBSyncPriority.HIGH].Count > 0)
			{
				DbSyncRequest? request;
				_ = _requestQueue[(Int32)DBSyncPriority.HIGH].TryDequeue(out request);
				if (request == null)
				{
					_ = _requestQueue[(Int32)DBSyncPriority.NORMAL].TryDequeue(out request);
				}
				if (request == null)
				{
					_ = _requestQueue[(Int32)DBSyncPriority.LOW].TryDequeue(out request);
				}
				if (request == null)
				{
					Thread.Sleep(1);
					continue;
				}

				var charId = request.CharId;
				var isFinal = request.Final;
				var newTimestamps = request.Timestamp;
				if (!_syncTimestamps.TryGetValue(charId, out var oldTimestamps))
				{
					oldTimestamps = new SyncTimestamps();
					_syncTimestamps.TryAdd(charId, oldTimestamps);
				}

				if (request.DbSyncEquipment != null)
				{
					if (oldTimestamps.Equipment.Ticks < newTimestamps.Ticks)
					{
						oldTimestamps.Equipment = newTimestamps;
						_databaseManager.CharacterManager.SyncEquipment(charId, request.DbSyncEquipment.EquipmentData).ExecuteDbSync(isFinal, _masterRpcChannel, SyncFlags.EQUIPMENT, charId);
					}
				}
				if (request.DbSyncInventory != null)
				{
					if (oldTimestamps.Inventory.Ticks < newTimestamps.Ticks)
					{
						oldTimestamps.Inventory = newTimestamps;
						_databaseManager.CharacterManager.SyncInventory(charId, request.DbSyncInventory.InventoryData).ExecuteDbSync(isFinal, _masterRpcChannel, SyncFlags.INVENTORY, charId);
					}
				}
				if (request.DbSyncSkills != null)
				{
					if (oldTimestamps.Skills.Ticks < newTimestamps.Ticks)
					{
						oldTimestamps.Skills = newTimestamps;
						_databaseManager.CharacterManager.SyncSkills(charId, request.DbSyncSkills.SkillData).ExecuteDbSync(isFinal, _masterRpcChannel, SyncFlags.SKILLS, charId);
					}
				}
				if (request.DbSyncQuickSlotBar != null)
				{
					if (oldTimestamps.QuickSlotBar.Ticks < newTimestamps.Ticks)
					{
						oldTimestamps.QuickSlotBar = newTimestamps;
						_databaseManager.CharacterManager.SyncLinks(charId, request.DbSyncQuickSlotBar.QuickSlotData).ExecuteDbSync(isFinal, _masterRpcChannel, SyncFlags.QUICKSLOT, charId);
					}
				}
				if (request.DbSyncLocation != null)
				{
					if (oldTimestamps.Location.Ticks < newTimestamps.Ticks)
					{
						oldTimestamps.Location = newTimestamps;
						_databaseManager.CharacterManager.SyncLocation(charId, request.DbSyncLocation).ExecuteDbSync(isFinal, _masterRpcChannel, SyncFlags.LOCATION, charId);
					}
				}
				if (request.DbSyncStats != null)
				{
					if (oldTimestamps.Stats.Ticks < newTimestamps.Ticks)
					{
						oldTimestamps.Stats = newTimestamps;
						_databaseManager.CharacterManager.SyncStats(charId, request.DbSyncStats).ExecuteDbSync(isFinal, _masterRpcChannel, SyncFlags.STATS, charId);
					}
				}
				if (request.DbSyncStatus != null)
				{
					if (oldTimestamps.Status.Ticks < newTimestamps.Ticks)
					{
						oldTimestamps.Status = newTimestamps;
						_databaseManager.CharacterManager.SyncStatus(charId, request.DbSyncStatus).ExecuteDbSync(isFinal, _masterRpcChannel, SyncFlags.STATUS, charId);
					}
				}

				//todo: tell master server to release lock, but when....
				//have to do SOMETHING when all of the Sync* functions are done, SOMEHOW
				//and Final boolean is set

			}
		}

		public void Stop()
		{
			_running = false;
		}
	}

	internal static class DbSyncUtils
	{
		public static async void ExecuteDbSync(this Task<int> task, bool isFinal, GrpcChannel grpcChannel, SyncFlags flag, int charId)
		{
			try
			{
				var result = await task;
				if (isFinal)
				{
					if (result == 1)
					{
						var client = new CharacterMaster.CharacterMasterClient(grpcChannel);
						var serverId = ServerConfig.Get().GeneralSettings.ServerId;
						var reply = await client.SetCharacterSyncStatusAsync(new SetCharacterSyncStatusRequest { CharId = (UInt32)charId, ServerId = (UInt32)serverId, SyncFlags = (UInt32)flag });
						if (reply.Status != 1)
							throw new Exception("Problem in syncing");
						Serilog.Log.Information($"{flag} synced for char id {charId}");
					}
					else
					{
						throw new Exception("failed sql??");
					}
				}
			}
			catch (Exception e)
			{
				Serilog.Log.Error(e.ToString());
				throw;
			}
		}
	}
}
