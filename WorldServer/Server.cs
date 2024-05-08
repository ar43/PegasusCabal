using Grpc.Net.Client;
using LibPegasus.Crypt;
using LibPegasus.JSON;
using Npgsql;
using Serilog;
using Shared.Protos;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using WorldServer.DB;
using WorldServer.Logic;
using WorldServer.Logic.World;

namespace WorldServer
{
	internal class Server
	{
		bool _running = false;
		bool _started = false;
		TcpListener _listener;
		IPAddress _externalIp;

		GrpcChannel _masterRpcChannel;

		public static readonly int MAX_CLIENTS = 1024; //TODO

		ConcurrentQueue<Client> _awaitingClients = new();
		List<Client> _clients = new();
		ConcurrentQueue<SessionChangeData> _pendingSessionChanges = new();
		XorKeyTable _xorKeyTable = new();

		DatabaseManager _databaseManager;

		InstanceManager _instanceManager;

		bool[] _clientIndexSpace = new bool[UInt16.MaxValue + 1];

		public Server()
		{
			var cfg = ServerConfig.Get("world1");

			Log.Information($"Channel id: {cfg.GeneralSettings.ChannelId}");

			Log.Information("Connecting to MasterServer...");
			var handler = new SocketsHttpHandler
			{
				ConnectTimeout = Timeout.InfiniteTimeSpan,
				PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
				KeepAlivePingDelay = TimeSpan.FromSeconds(60),
				KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
				KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
				PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
			};

			_masterRpcChannel = GrpcChannel.ForAddress("https://localhost:7190", new GrpcChannelOptions
			{
				HttpHandler = handler
			});

			_masterRpcChannel.ConnectAsync().Wait();

			var externalIpTask = LibPegasus.Utils.Utility.GetExternalIpAddress();
			externalIpTask.Wait();
			_externalIp = externalIpTask.Result ?? IPAddress.Loopback;

			Log.Information("Connected to MasterServer");

			var ipEndPoint = new IPEndPoint(IPAddress.Any, cfg.ConnectionSettings.Port);
			_listener = new(ipEndPoint);

			_databaseManager = new DatabaseManager();
			_instanceManager = new InstanceManager();
		}
		
		void SendHeartbeat()
		{
			Log.Information("Starting to send heartbeats...");
			const double heartbeatTime = 5.0;
			DateTime lastUpdate = DateTime.MinValue;

			while (_masterRpcChannel != null)
			{
				if (DateTime.UtcNow.Ticks - lastUpdate.Ticks >= TimeSpan.FromSeconds(heartbeatTime).Ticks)
				{
					var cfg = ServerConfig.Get();
					var client = new ChannelMaster.ChannelMasterClient(_masterRpcChannel);
					try
					{
						_masterRpcChannel.ConnectAsync().Wait();
						var reply = client.Heartbeat(new WorldHeartbeatRequest { ServerId = (uint)cfg.GeneralSettings.ServerId, ChannelId = (uint)cfg.GeneralSettings.ChannelId, Ip = _externalIp.ToString(), Port = (uint)cfg.ConnectionSettings.Port });
						lastUpdate = DateTime.UtcNow;
						//Log.Debug("World Heartbeat return code from MasterServer: " + (InfoCodeWorldHeartbeat)reply.InfoCode);
					}
					catch (Grpc.Core.RpcException)
					{
						//TODO
						Log.Warning("Failed to send heartbeat. Retrying.");
					}
				}
				Thread.Sleep(1);
			}

			Log.Information("Exited SendHeartbeat");
		}

		UInt16 GetAvailableUserIndex()
		{
			for (int i = 0; i < _clientIndexSpace.Length; i++)
			{
				if (_clientIndexSpace[i] == false)
				{
					_clientIndexSpace[i] = true;
					return (UInt16)i;
				}
			}

			throw new Exception("Server full");
		}

		void FreeUserIndex(UInt16 index)
		{
			_clientIndexSpace[(int)index] = false;
		}

		void AcceptNewConnections()
		{
			Log.Information("Listening for connections...");


			while (_listener != null)
			{
				TcpClient tcpClient = _listener.AcceptTcpClient();
				if (tcpClient != null)
				{
					Log.Debug("Client connected");
					_awaitingClients.Enqueue(new Client(tcpClient, _xorKeyTable, _masterRpcChannel, _databaseManager, _instanceManager));
				}
			}

			Log.Information("Listener is closed, exiting AcceptNewConnections()");
		}

		void Start()
		{
			_listener.Start();
			_started = true;

			Log.Information("Started Pegasus LoginServer");

			Task.Factory.StartNew(() => AcceptNewConnections(), TaskCreationOptions.LongRunning);
			Task.Factory.StartNew(() => SendHeartbeat(), TaskCreationOptions.LongRunning);
			Task.Factory.StartNew(() => SessionChangeListener(), TaskCreationOptions.LongRunning);

			_running = true;

			RunTest();
		}

		async Task SessionChangeListener()
		{
			var cfg = ServerConfig.Get();

			await using var conn = new NpgsqlConnection(cfg.DatabaseSettings.ConnString);
			await conn.OpenAsync();

			//e.Payload is string representation of JSON we constructed in NotifyOnDataChange() function
			conn.Notification += (o, e) => SessionChangeHandler(e.Payload);

			await using (var cmd = new NpgsqlCommand("LISTEN sessionchange;", conn))
				cmd.ExecuteNonQuery();

			while (true)
				conn.Wait(); // wait for events
		}

		void SessionChangeHandler(string payload)
		{
			var cfg = ServerConfig.Get();
			Log.Debug("Received session notification: " + payload);
			var sessionChangeData = JsonSerializer.Deserialize<SessionChangeData>(payload);
			if (sessionChangeData.table != "sessions" || sessionChangeData.data.channel_id != cfg.GeneralSettings.ChannelId || sessionChangeData.data.server_id != cfg.GeneralSettings.ServerId)
			{
				return;
			}
			_pendingSessionChanges.Enqueue(sessionChangeData);
		}

		private void RunTest()
		{
		}

		void ProcessSessionChanges()
		{
			while (!_pendingSessionChanges.IsEmpty)
			{
				_pendingSessionChanges.TryDequeue(out var sessionChanges);
				if (sessionChanges != null)
				{
					var userId = Convert.ToInt16(sessionChanges.data.user_id);
					var client = _clients.Find(x => x.ConnectionInfo.UserId == userId);
					if (client != null)
					{
						bool login = sessionChanges.action == "INSERT" ? true : false;
						var authKey = Convert.ToUInt32(sessionChanges.data.auth_key);
						var accountId = Convert.ToUInt32(sessionChanges.data.account_id);
						if (login)
						{
							client.OnLogin(authKey, accountId);
						}
						else
						{
							client.OnLogout(authKey, accountId);
						}
					}
				}
			}
		}

		public void Run()
		{
			if (!_started)
			{
				Start();
			}

			while (_running)
			{
				AddNewClients();
				ProcessClients();
				RemoveClients();
				ProcessSessionChanges();
				Thread.Sleep(1);
			}

			Quit();
		}

		private void RemoveClients()
		{
			for (var i = _clients.Count - 1; i >= 0; i--)
			{
				if (_clients[i].Dropped)
				{
					_clients[i].TcpClient.Close();
					FreeUserIndex(_clients[i].ConnectionInfo.UserId);
					if (_clients[i].Character != null && _clients[i].Character.Location.Instance != null)
						_clients[i].Character.Location.Instance.RemoveClient(_clients[i], Enums.DelUserType.LOGOUT);
					_clients.RemoveAt(i);
				}
			}
		}

		private void ProcessClients()
		{
			foreach (var client in _clients)
			{
				client.ReceiveData();
				client.Update();
				client.SendData();
			}
		}

		private void AddNewClients()
		{
			while (!_awaitingClients.IsEmpty)
			{
				_awaitingClients.TryDequeue(out Client? client);
				if (client != null)
				{
					Log.Debug("Added Client from awaiting to non-awaiting");
					_clients.Add(client);
					client.OnClientAccept(GetAvailableUserIndex());
				}
			}
		}

		private void Quit()
		{
			_listener.Stop();
		}
	}
}
