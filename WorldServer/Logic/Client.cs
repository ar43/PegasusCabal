using Grpc.Net.Client;
using LibPegasus.Crypt;
using LibPegasus.Packets;
using LibPegasus.Utils;
using Serilog;
using Shared.Protos;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using WorldServer.DB;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;
using WorldServer.Logic.CharData;
using WorldServer.Logic.ClientData;
using WorldServer.Logic.World;
using WorldServer.Packets;

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
		private InstanceManager _instanceManager;

		public Character? Character;
		public Account? Account;

		readonly double TIMEOUT_SECONDS = 99999.0;

		DateTime timeClientAccepted;

		private bool _busy = false;

		internal bool Dropped { get; private set; } = false;

		public Client(TcpClient tcpClient, XorKeyTable xorKeyTable, GrpcChannel masterChannel, DatabaseManager databaseManager, InstanceManager instanceManager)
		{
			TcpClient = tcpClient;
			PacketManager = new PacketManager();
			Encryption = new(xorKeyTable);
			_masterRpcChannel = masterChannel;

			var remoteEndPoint = TcpClient.Client.RemoteEndPoint as IPEndPoint;
			Ip = remoteEndPoint.Address.ToString();

			_databaseManager = databaseManager;
			_instanceManager = instanceManager;
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
							Utility.PrintByteArray(packetBytes, packetLen, "received decrypted");
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

							Utility.PrintByteArray(packetBytes, packetLen, "decrypted");
						}
					}
				}
				else if(length == 0)
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

			var actions = PacketManager.ReceiveAll(ConnectionInfo.IsAuthenticated(), _instanceManager);

			if (actions != null)
			{
				while (actions.Count > 0)
				{
					var action = actions.Dequeue();
					//add try and catch - drop player on exception
					action(this);
				}
			}

			var time = DateTime.UtcNow;

			if (time.Ticks - timeClientAccepted.Ticks >= TimeSpan.FromSeconds(TIMEOUT_SECONDS).Ticks)
			{
				//timeout
				Disconnect("timeout", ConnState.TIMEOUT);
				return;
			}
		}

		internal void Disconnect(string reason, ConnState newState)
		{
			Dropped = true;
			Log.Warning($"called Disconnect on client id {ConnectionInfo.UserId} with reason: {reason}");
			ConnectionInfo.ConnState = newState;
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
			var reply = await client.GetChatServerInfoAsync(new GetChatServerInfoRequest { IsLocalhost = isLocalhost});
			return reply;
		}

		internal async Task<GetMyCharactersReply> SendCharListRequest()
		{
			var client = new CharacterMaster.CharacterMasterClient(_masterRpcChannel);
			var serverId = ServerConfig.Get().GeneralSettings.ServerId;
			var reply = await client.GetMyCharactersAsync(new GetMyCharactersRequest { AccountId = ConnectionInfo.AccountId, ServerId = (UInt32)serverId });
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
	}
}
