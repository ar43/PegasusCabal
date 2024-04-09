using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Collections.Concurrent;
using LoginServer.DB;
using Serilog;
using LoginServer.Crypt;
using LoginServer.Logic;
using System.Text.Json;
using Grpc.Net.Client;
using LibPegasus.DB;
using LibPegasus.Enums;
using System.Threading.Channels;
using Shared.Protos;

namespace LoginServer
{
    internal class Server
	{
		bool _running = false;
		bool _started = false;
		TcpListener _listener;

		GrpcChannel _masterChannel;

		internal static string LOGIN_SECRET { get; private set; }
		public static readonly int MAX_CLIENTS = 1024;

		ConcurrentQueue<Client> _awaitingClients = new();
		List<Client> _clients = new();
		XorKeyTable _xorKeyTable = new();

		DatabaseManager _databaseManager;

		bool[] _clientIndexSpace = new bool[MAX_CLIENTS];

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

			_masterChannel = GrpcChannel.ForAddress("https://localhost:7190", new GrpcChannelOptions
			{
				HttpHandler = handler
			});

			_masterChannel.ConnectAsync().Wait();

			Log.Information("Connected to MasterServer");

			var ipEndPoint = new IPEndPoint(IPAddress.Any, cfg.ConnectionSettings.Port);
			_listener = new(ipEndPoint);

			_databaseManager = new DatabaseManager(cfg.DatabaseSettings.ConnString);

			ReadSecret();
		}

		UInt16 GetAvailableUserIndex()
		{
			for(int i = 0; i < _clientIndexSpace.Length; i++)
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

		void ReadSecret()
		{
			string workingDirectory = Environment.CurrentDirectory;
			string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

			string secretFile = projectDirectory + "\\secret.cfg";
			LOGIN_SECRET = File.ReadAllText(secretFile);
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
					_awaitingClients.Enqueue(new Client(tcpClient, _xorKeyTable));
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

			_running = true;

			RunTest();
		}

		private async void RunTest()
		{
			Log.Debug("Running tests...");
			var code = await _databaseManager.AccountManager.RequestRegister("dummy", "dummypass");
			Log.Debug("Registration return code: " + code);

			var token = await _databaseManager.AccountManager.RequestLogin("dummy", "dummypass", DateTime.UtcNow.Ticks, [1, 2, 3, 4], Server.LOGIN_SECRET);
			var token_bad = await _databaseManager.AccountManager.RequestLogin("dummy", "dummypassfewfwe", DateTime.UtcNow.Ticks, [1, 2, 3, 4], Server.LOGIN_SECRET);
			if (token != false && token_bad == false)
			{
				Log.Debug("Login test success");
			}
			else
			{
				Log.Debug("Login test failed");
				throw new ApplicationException("LoginServer test failed");
			}

			var client = new RegisterAccount.RegisterAccountClient(_masterChannel);
			var reply = await client.RegisterAsync(new RegisterAccountRequest { Username = "dummybla", Password = "dummypa"});
			Log.Debug("Account Registration return code from MasterServer: " + (InfoCodeLS)reply.InfoCode);

		}

		public void Run()
		{
			if(!_started) 
			{
				Start();
			}

			while(_running)
			{
				AddNewClients();
				ProcessClients(_databaseManager.AccountManager);
				RemoveClients();
				Thread.Sleep(1);
			}

			Quit();
		}

		private void RemoveClients()
		{
			for(var i = _clients.Count - 1; i >= 0; i--)
			{
				if (_clients[i].Dropped)
				{
					_clients[i].TcpClient.Close();
					_clients.RemoveAt(i);
				}
			}
		}

		private void ProcessClients(AccountManager accountManager)
		{
			foreach(var client in _clients)
			{
				client.ReceiveData();
				client.Update(accountManager);
				client.SendData();
			}
		}

		private void AddNewClients()
		{
			while(!_awaitingClients.IsEmpty)
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
