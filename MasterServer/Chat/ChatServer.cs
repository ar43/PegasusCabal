using Grpc.Net.Client;
using LibPegasus.Crypt;
using LibPegasus.Enums;
using LibPegasus.JSON;
using MasterServer.DB;
using Npgsql;
using Serilog;
using Shared.Protos;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace MasterServer.Chat
{
	public class ChatServer
	{
		bool _running = false;
		bool _started = false;
		TcpListener _listener;

		ConcurrentQueue<ChatClient> _awaitingClients = new();
		List<ChatClient> _clients = new();
		XorKeyTable _xorKeyTable = new();
		ConcurrentQueue<SessionChangeData> _pendingSessionChanges = new();

		public IPAddress Ip { get; }
		public UInt16 Port { get; }

		private readonly DatabaseManager _databaseManager;

		bool[] _clientIndexSpace = new bool[UInt16.MaxValue + 1];

		public ChatServer(IConfiguration configuration)
		{
			var ipEndPoint = new IPEndPoint(IPAddress.Any, Convert.ToUInt16(configuration["ChatServerPort"]));
			Ip = IPAddress.Any;
			Port = Convert.ToUInt16(configuration["ChatServerPort"]);
			_listener = new(ipEndPoint);

			_databaseManager = new DatabaseManager(configuration);
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
					_awaitingClients.Enqueue(new ChatClient(tcpClient, _xorKeyTable));
				}
			}

			Log.Information("Listener is closed, exiting AcceptNewConnections()");
		}

		void Start()
		{
			_listener.Start();
			_started = true;

			Log.Information("Started Pegasus ChatServer");

			Task.Factory.StartNew(() => AcceptNewConnections(), TaskCreationOptions.LongRunning);
			//Task.Factory.StartNew(() => SessionChangeListener(), TaskCreationOptions.LongRunning);

			_running = true;

			RunTest();
		}

		//async Task SessionChangeListener()
		//{
		//	//var cfg = ServerConfig.Get();

		//	await using var conn = new NpgsqlConnection(cfg.DatabaseSettings.ConnString);
		//	await conn.OpenAsync();

		//	//e.Payload is string representation of JSON we constructed in NotifyOnDataChange() function
		//	conn.Notification += (o, e) => SessionChangeHandler(e.Payload);

		//	await using (var cmd = new NpgsqlCommand("LISTEN loginsessionchange;", conn))
		//		cmd.ExecuteNonQuery();

		//	while (true)
		//		conn.Wait(); // wait for events
		//}

		void SessionChangeHandler(string payload)
		{
			Log.Debug("Received session notification: " + payload);
			var sessionChangeData = JsonSerializer.Deserialize<SessionChangeData>(payload);
			//TODO: do something with serverId and channelId
			if (sessionChangeData.table != "sessions")
			{
				return;
			}
			_pendingSessionChanges.Enqueue(sessionChangeData);
		}

		private void RunTest()
		{
			
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
				_awaitingClients.TryDequeue(out ChatClient? client);
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
