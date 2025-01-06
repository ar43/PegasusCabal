using Grpc.Net.Client;
using LibPegasus.Crypt;
using LibPegasus.Packets;
using Serilog;
using Shared.Protos;
using System.Net;
using System.Net.Sockets;
using WorldServer.DB;
using WorldServer.DB.Sync;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.DbSyncData;
using WorldServer.Logic.ClientData;
using WorldServer.Logic.WorldRuntime;
using WorldServer.Packets;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic
{
	internal class Client
	{
		public TcpClient TcpClient { private set; get; }

		public ConnectionInfo? ConnectionInfo { private set; get; }
		public string Ip { private set; get; }

		public PacketManager PacketManager;

		public Encryption Encryption;

		GrpcChannel _masterRpcChannel;

		private DatabaseManager _databaseManager;
		public World World;

		public Character? Character;
		public Account? Account;

		public LibPegasus.Utils.Timer? TimerHeartbeat = null; //set to 40
		public LibPegasus.Utils.Timer? TimerHeartbeatTimeout = null; //set this to null on successfull heartbeat
		public LibPegasus.Utils.Timer? TimerDbSync = null;

		DateTime timeClientAccepted;

		private bool _busy = false;

		internal bool Dropped { get; private set; } = false;

		public Client(TcpClient tcpClient, XorKeyTable xorKeyTable, GrpcChannel masterChannel, DatabaseManager databaseManager, World world)
		{
			TcpClient = tcpClient;
			PacketManager = new PacketManager();
			Encryption = new(xorKeyTable);
			_masterRpcChannel = masterChannel;

			var remoteEndPoint = TcpClient.Client.RemoteEndPoint as IPEndPoint;
			Ip = remoteEndPoint.Address.ToString();

			_databaseManager = databaseManager;
			World = world;
		}

		internal void OnClientAccept(UInt16 userIndex)
		{
			timeClientAccepted = DateTime.UtcNow;
			UInt32 unixTime = (UInt32)((DateTimeOffset)timeClientAccepted).ToUnixTimeSeconds();
			ConnectionInfo = new(userIndex, unixTime);
		}

		internal void ReceiveData()
		{
			if (!TcpClient.Connected || Dropped)
			{
				return;
			}

			var stream = TcpClient.GetStream();

			if (stream != null && stream.CanRead && stream.DataAvailable)
			{
				Byte[] bytes = new Byte[1024];
				var length = stream.Read(bytes, 0, bytes.Length);
				if (length != 0)
				{
					//PrintByteArray(bytes, length, "received encrypted");
					var i = 0;

					if (PacketManager.DanglingPacket != null)
					{
						var remaining = PacketManager.DanglingPacket.GetRemaining();

						var amountToCopy = Math.Min(remaining, length);

						remaining = PacketManager.DanglingPacket.Add(bytes, amountToCopy);

						i += amountToCopy;

						if (remaining == 0)
						{
							var packetLen = PacketManager.DanglingPacket.PacketLen;
							byte[] packetBytes = new byte[packetLen];
							Array.Copy(PacketManager.DanglingPacket.DanglingData, 0, packetBytes, 0, packetLen);
							var opcode = Encryption.Decrypt(packetBytes);

							PacketManager.EnqueuePacket(opcode, new Queue<byte>(packetBytes));

							PacketManager.DanglingPacket = null;
							//Utility.PrintByteArray(packetBytes, packetLen, "received decrypted");
						}

					}

					while (i < length)
					{
						if (length - i < 4)
						{
							throw new NotImplementedException("length-i < 4 on packet read");
						}

						var span = new Span<byte>(bytes, i, length - i);
						var packetLen = Encryption.GetPacketSize(span);
						Log.Debug($"packetLen decrypted: {packetLen}");

						if (packetLen < Encryption.C2S_HEADER_SIZE || packetLen > DanglingPacket.MAX_C2S_PACKET_LEN)
						{
							throw new OverflowException("ReceiveData - packetLen > MAX_C2S_PACKET_LEN");
						}

						if (packetLen > length - i)
						{
							PacketManager.DanglingPacket = new(bytes, i, length, packetLen);
							break;
						}
						else
						{
							byte[] packetBytes = new byte[packetLen];
							Array.Copy(bytes, i, packetBytes, 0, packetLen);
							i += packetLen;

							var opcode = Encryption.Decrypt(packetBytes);

							PacketManager.EnqueuePacket(opcode, new Queue<byte>(packetBytes));

							//Utility.PrintByteArray(packetBytes, packetLen, "decrypted");
						}
					}
				}
				else if (length == 0)
				{
					throw new NotImplementedException();
				}
			}
		}

		internal async Task<(Character?, int)> LoadCharacter(UInt32 characterId)
		{
			return await _databaseManager.CharacterManager.GetCharacter(characterId);
		}
		internal void SendData()
		{
			if (!TcpClient.Connected || !PacketManager.OutputQueued())
			{
				return;
			}

			var stream = TcpClient.GetStream();
			while (PacketManager.OutputQueued())
			{
				if (stream.CanWrite)
				{
					byte[] send = Encryption.Encrypt(PacketManager.GetOutboundPacket());

					try
					{
						stream.Write(send);
					}
					catch (IOException)
					{
						Disconnect("Disconnect - cannot send data", ConnState.DISCONNECTED);
					}
					//PrintByteArray(send, send.Length, "encrypted sent");
				}
			}
		}

		internal void OnLogin(UInt32 authKey, UInt32 accountId)
		{
			if (authKey != ConnectionInfo.AuthKey)
			{
				throw new NotImplementedException("wrong auth key");
			}
			if (ConnectionInfo.ConnState == Enums.ConnState.AWAITING)
			{
				ConnectionInfo.SetAccountId(accountId);
				ConnectionInfo.ConnState = Enums.ConnState.CONNECTED;
				return;
			}
		}

		internal void OnLogout(UInt32 authKey, UInt32 accountId)
		{
			if (authKey != ConnectionInfo.AuthKey || accountId != ConnectionInfo.AccountId)
			{
				//is not the guy we're trying to log out
				return;
			}
			Disconnect("force logout - session deleted", ConnState.KICKED);
			//TODO: proper logout
		}

		internal void Update()
		{
			if (_busy || Dropped)
				return;

			if (ConnectionInfo.ConnState == Enums.ConnState.AWAITING)
			{
				//TODO: Add timeout
				return;
			}

			var actions = PacketManager.ReceiveAll(ConnectionInfo.IsAuthenticated());

			if (actions != null)
			{
				while (actions.Count > 0)
				{
					var action = actions.Dequeue();
					//add try and catch - drop player on exception
					action(this);
				}
			}

			if (TimerHeartbeat != null && TimerHeartbeat.Tick())
			{
				if (Character != null)
				{
					var heartbeatPacket = new REQ_Heartbeat();
					PacketManager.Send(heartbeatPacket);
					TimerHeartbeatTimeout = new LibPegasus.Utils.Timer(DateTime.UtcNow, 10000.0, false);
				}
				else
				{
					TimerHeartbeat = null;
				}
			}

			if (TimerHeartbeatTimeout != null && TimerHeartbeatTimeout.Tick())
			{
				if (Character != null)
				{
					Disconnect("Heartbeat timeout", ConnState.TIMEOUT);
				}
				else
				{
					TimerHeartbeat = null;
				}
			}

			if (Character != null && Character.Location.Instance != null)
			{
				UpdateInWorld();
			}


		}

		private void UpdateInWorld()
		{
			if (Character.Location.Movement.IsDeadReckoning)
			{
				var oldX = Character.Location.Movement.X;
				var oldY = Character.Location.Movement.Y;
				Character.Location.Movement.DeadReckoning();
				var newX = Character.Location.Movement.X;
				var newY = Character.Location.Movement.Y;

				if (oldX != newX || oldY != newY)
				{
					if (Character.Location.Instance.CheckTileMoveDisable((UInt16)newX, (UInt16)newY))
					{
						Character.Location.Movement.IllegalMovementCounter++;
					}
					if (Character.Location.Movement.IllegalMovementCounter >= 1)
					{
						Disconnect("IllegalMovementCounter >= 1", ConnState.ERROR);
						return;
					}
				}
			}
			while (Character.Stats.LvlUpEventQueue.Count > 0)
			{
				var packet_update = new NFY_UpdateDatas(UpdateType.UT_LEVEL, (ushort)Character.Stats.LvlUpEventQueue.Dequeue(), 0);
				PacketManager.Send(packet_update);

				var packet_nfy = new NFY_ChartrEvent(CharEvent.EVT_LEVELUP, Character.Id);
				this.BroadcastNearby(packet_nfy);
			} 
		}

		internal void Error(string funcName, string message)
		{
			Disconnect($"{funcName} - {message}", ConnState.ERROR);
		}

		internal void Disconnect(string reason, ConnState newState)
		{
			if (Dropped)
				return;
			Dropped = true;
			Log.Warning($"called Disconnect on client id {ConnectionInfo.UserId} with reason: {reason}");
			ConnectionInfo.ConnState = newState;
			Character?.Sync(DBSyncPriority.HIGH);

			//todo - send session timeout

		}

		internal async Task<SessionReply> SendLoginSessionRequest(UInt32 authKey, UInt16 userId, Byte channelId, Byte serverId)
		{
			var client = new AuthMaster.AuthMasterClient(_masterRpcChannel);
			var reply = await client.CreateLoginSessionAsync(new SessionRequest { AuthKey = authKey, UserId = userId, ChannelId = channelId, ServerId = serverId, AccountId = ConnectionInfo.AccountId });
			return reply;
		}

		internal async Task<CreateCharacterReply> SendCharCreationRequest(Character chr, byte slot)
		{
			var client = new CharacterMaster.CharacterMasterClient(_masterRpcChannel);
			var serverId = ServerConfig.Get().GeneralSettings.ServerId;
			var reply = await client.CreateCharacterAsync(new CreateCharacterRequest { Style = chr.Style.Serialize(), Slot = slot, AccountId = ConnectionInfo.AccountId, ServerId = (UInt32)serverId, JoinNoviceGuild = false, Name = chr.Name });
			return reply;
		}

		internal async Task<GetChatServerInfoReply> RequestChatServerInfo()
		{
			var client = new ChatMaster.ChatMasterClient(_masterRpcChannel);
			bool isLocalhost = Ip == "127.0.0.1";
			var reply = await client.GetChatServerInfoAsync(new GetChatServerInfoRequest { IsLocalhost = isLocalhost });
			return reply;
		}

		internal async Task<GetMyCharactersReply> SendCharListRequest()
		{
			var client = new CharacterMaster.CharacterMasterClient(_masterRpcChannel);
			var serverId = ServerConfig.Get().GeneralSettings.ServerId;
			var reply = await client.GetMyCharactersAsync(new GetMyCharactersRequest { AccountId = ConnectionInfo.AccountId, ServerId = (UInt32)serverId });
			return reply;
		}

		internal async Task<CharacterSyncStatusReply> IsCharacterSynced(int charId)
		{
			var client = new CharacterMaster.CharacterMasterClient(_masterRpcChannel);
			var serverId = ServerConfig.Get().GeneralSettings.ServerId;
			var reply = await client.CharacterSyncStatusAsync(new CharacterSyncStatusRequest { CharId = (UInt32)charId, ServerId = (UInt32)serverId });
			return reply;
		}

		internal async Task<(string, DateTime?)> GetSubPasswordData()
		{
			var reply = await _databaseManager.SubpassManager.GetSubPasswordData((Int32)ConnectionInfo.AccountId);
			return reply;
		}

		internal async Task<bool> SetSubPassword(string subpass)
		{
			var reply = await _databaseManager.SubpassManager.SetSubpass((Int32)ConnectionInfo.AccountId, subpass);
			return reply;
		}

		internal async Task<bool> SetSubPasswordVerificationDate()
		{
			var reply = await _databaseManager.SubpassManager.SetSubPasswordVerificationDate((Int32)ConnectionInfo.AccountId, DateTime.UtcNow);
			return reply;
		}

		public void BroadcastNearby(PacketS2C packet, bool excludeClient = false)
		{
			Character.Location.Instance.BroadcastNearby(this, packet, excludeClient);
		}

		internal void DbSync(SyncManager syncManager)
		{
			//do it periodically, eg every 5 seconds
			//check flags to see what to update
			//send DbSyncRequest object which contains attributes depending on DbSyncFlags (DbSyncInventory... etc)
			//add bool to DbSyncRequest to notify master server on data save.
			//add timestamp to DbSyncRequest
			var cfg = ServerConfig.Get();
			if (Character != null && cfg.DatabaseSettings.EnableDbSync)
			{
				if (Character.SyncPending > DBSyncPriority.NONE || TimerDbSync != null && TimerDbSync.Tick())
				{
					DBSyncPriority globalPrio = Character.SyncPending;
					DBSyncPriority highestPrio = DBSyncPriority.NONE;
					bool isFinal = Dropped || Character.UninitOnSync;

					DbSyncEquipment? dbSyncEquipment = null;
					DbSyncInventory? dbSyncInventory = null;
					DbSyncLocation? dbSyncLocation = null;
					DbSyncQuickSlotBar? dbSyncQuickSlotBar = null;
					DbSyncSkills? dbSyncSkills = null;
					DbSyncStats? dbSyncStats = null;
					DbSyncStatus? dbSyncStatus = null;

					if (Character.Inventory.SyncPending > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (Character.Inventory.SyncPending > highestPrio)
							highestPrio = Character.Inventory.SyncPending;
						dbSyncInventory = new(Character.Inventory.GetProtobuf(), Character.Inventory.Alz);
					}
					if (Character.Equipment.SyncPending > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (Character.Equipment.SyncPending > highestPrio)
							highestPrio = Character.Equipment.SyncPending;
						dbSyncEquipment = new(Character.Equipment.GetProtobuf());
					}
					if (Character.Location.SyncPending > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (Character.Location.SyncPending > highestPrio)
							highestPrio = Character.Location.SyncPending;
						dbSyncLocation = Character.Location.GetDB();
					}
					if (Character.QuickSlotBar.SyncPending > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (Character.QuickSlotBar.SyncPending > highestPrio)
							highestPrio = Character.QuickSlotBar.SyncPending;
						dbSyncQuickSlotBar = new(Character.QuickSlotBar.GetProtobuf());
					}
					if (Character.Skills.SyncPending > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (Character.Skills.SyncPending > highestPrio)
							highestPrio = Character.Skills.SyncPending;
						dbSyncSkills = new(Character.Skills.GetProtobuf());
					}
					if (Character.Stats.SyncPending > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (Character.Stats.SyncPending > highestPrio)
							highestPrio = Character.Stats.SyncPending;
						dbSyncStats = Character.Stats.GetDB();
					}
					if (Character.Status.SyncPending > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (Character.Status.SyncPending > highestPrio)
							highestPrio = Character.Status.SyncPending;
						dbSyncStatus = Character.Status.GetDB();
					}

					if (highestPrio > DBSyncPriority.NONE || globalPrio > DBSyncPriority.NONE)
					{
						if (globalPrio > DBSyncPriority.NONE)
							highestPrio = globalPrio;
						DbSyncRequest dbSyncRequest = new(highestPrio, Character.Id, isFinal);
						dbSyncRequest.DbSyncInventory = dbSyncInventory;
						dbSyncRequest.DbSyncQuickSlotBar = dbSyncQuickSlotBar;
						dbSyncRequest.DbSyncSkills = dbSyncSkills;
						dbSyncRequest.DbSyncStats = dbSyncStats;
						dbSyncRequest.DbSyncStatus = dbSyncStatus;
						dbSyncRequest.DbSyncEquipment = dbSyncEquipment;
						dbSyncRequest.DbSyncLocation = dbSyncLocation;
						Log.Information($"Sent sync request (prio: {highestPrio}, char: {Character.Id}, final: {isFinal})");
						syncManager.AddToQueue(dbSyncRequest, highestPrio);
						Character.ClearSync();
						if (Character.UninitOnSync)
							Character = null;
					}
				}
			}
		}

		public bool isGm()
		{
			if (Character == null)
			{
				return false;
			}

#if !DEBUG
			if (Character.Nation != NationCode.NATION_GM)
			{
				return false;
			}
#endif

			return true;
		}
	}
}
