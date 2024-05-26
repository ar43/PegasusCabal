using Grpc.Net.Client;
using LibPegasus.Crypt;
using LibPegasus.JSON;
using LoginServer.DB;
using LoginServer.Logic;
using Npgsql;
using Serilog;
using Shared.Protos;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace LoginServer
{
	internal class Server
	{
		bool _running = false;
		bool _started = false;
		TcpListener _listener;

		GrpcChannel _masterRpcChannel;

		ConcurrentQueue<Client> _awaitingClients = new();
		List<Client> _clients = new();
		XorKeyTable _xorKeyTable = new();
		ConcurrentQueue<SessionChangeData> _pendingSessionChanges = new();

		DatabaseManager _databaseManager;

		bool[] _clientIndexSpace = new bool[UInt16.MaxValue + 1];

		public Server()
		{
			var cfg = ServerConfig.Get();

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

			Log.Information("Connected to MasterServer");

			var ipEndPoint = new IPEndPoint(IPAddress.Any, cfg.ConnectionSettings.Port);
			_listener = new(ipEndPoint);

			_databaseManager = new DatabaseManager(cfg.DatabaseSettings.ConnString);
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
					_awaitingClients.Enqueue(new Client(tcpClient, _xorKeyTable, _masterRpcChannel));
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

			await using (var cmd = new NpgsqlCommand("LISTEN loginsessionchange;", conn))
				cmd.ExecuteNonQuery();

			while (true)
				conn.Wait(); // wait for events
		}

		void SessionChangeHandler(string payload)
		{
			var cfg = ServerConfig.Get();
			Log.Debug("Received session notification: " + payload);
			var sessionChangeData = JsonSerializer.Deserialize<SessionChangeData>(payload);
			//TODO: do something with serverId and channelId
			if (sessionChangeData.table != "sessions")
			{
				return;
			}
			_pendingSessionChanges.Enqueue(sessionChangeData);
		}

		private async void RunTest()
		{
			var now = DateTime.Now;
			var utcnow = DateTime.UtcNow;
			var client = new AuthMaster.AuthMasterClient(_masterRpcChannel);
			var reply = await client.RegisterAsync(new RegisterAccountRequest { Username = "dummybla", Password = "dummypa" });
			reply = await client.RegisterAsync(new RegisterAccountRequest { Username = "test1", Password = "test1" });
			reply = await client.RegisterAsync(new RegisterAccountRequest { Username = "test2", Password = "test2" });
			//Log.Debug("Account Registration return code from MasterServer: " + (InfoCodeLS)reply.InfoCode);
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

		void ProcessSessionChanges()
		{
			while (!_pendingSessionChanges.IsEmpty)
			{
				_pendingSessionChanges.TryDequeue(out var sessionChanges);
				Log.Debug("1");
				if (sessionChanges != null)
				{
					var userId = Convert.ToInt16(sessionChanges.data.user_id);
					var client = _clients.Find(x => x.ClientInfo.UserId == userId);
					Log.Debug($"trying to find client {userId}");
					if (client != null)
					{
						bool login = sessionChanges.action == "INSERT" ? true : false;
						var authKey = Convert.ToUInt32(sessionChanges.data.auth_key);
						var accountId = Convert.ToUInt32(sessionChanges.data.account_id);
						if (login)
						{
							Log.Debug($"calling linked login");
							client.OnLinkedLogin(authKey, accountId);
						}
						else
						{
							client.OnLinkedLogout(authKey, accountId);
						}
					}
				}
			}
		}

		private void RemoveClients()
		{
			for (var i = _clients.Count - 1; i >= 0; i--)
			{
				if (_clients[i].Dropped)
				{
					_clients[i].TcpClient.Close();
					FreeUserIndex(_clients[i].ClientInfo.UserId);
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
					client.OnConnect(GetAvailableUserIndex());
				}
			}
		}

		private void Quit()
		{
			_listener.Stop();
		}
	}
}
