using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Shared.Protos;
using LibPegasus.Enums;
using System.Threading;
using LibPegasus.Crypt;

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
		XorKeyTable _xorKeyTable = new();

		//DatabaseManager _databaseManager;

		bool[] _clientIndexSpace = new bool[MAX_CLIENTS];

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

			var externalIpTask = GetExternalIpAddress();
			externalIpTask.Wait();
			_externalIp = externalIpTask.Result ?? IPAddress.Loopback;

			Log.Information("Connected to MasterServer");

			var ipEndPoint = new IPEndPoint(IPAddress.Any, cfg.ConnectionSettings.Port);
			_listener = new(ipEndPoint);

			//_databaseManager = new DatabaseManager(cfg.DatabaseSettings.ConnString);
		}
		private static async Task<IPAddress?> GetExternalIpAddress()
		{
			var externalIpString = (await new HttpClient().GetStringAsync("http://icanhazip.com"))
				.Replace("\\r\\n", "").Replace("\\n", "").Trim();
			if (!IPAddress.TryParse(externalIpString, out var ipAddress)) return null;
			return ipAddress;
		}

		void SendHeartbeat()
		{
			Log.Information("Starting to send heartbeats...");
			const double heartbeatTime = 5.0;
			DateTime lastUpdate = DateTime.MinValue;

			while(_masterRpcChannel != null)
			{
				if(DateTime.UtcNow.Ticks - lastUpdate.Ticks >= TimeSpan.FromSeconds(heartbeatTime).Ticks)
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
			Task.Factory.StartNew(() => SendHeartbeat(), TaskCreationOptions.LongRunning);

			_running = true;

			RunTest();
		}

		private async void RunTest()
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
