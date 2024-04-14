using LoginServer.DB;
using LibPegasus.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using LoginServer.Opcodes.S2C;
using System.Net;
using LoginServer.Enums;
using LoginServer.Crypt;
using LoginServer.Opcodes;
using LoginServer.Packets;
using System.Net.Security;
using System.Security.Cryptography;
using LoginServer.Utils;
using Grpc.Net.Client;
using Shared.Protos;

namespace LoginServer.Logic
{
    internal class Client
    {
        public TcpClient TcpClient { private set; get; }

		public ClientInfo? ClientInfo { private set; get; }

        internal byte[] Ip { private set; get; }

        public PacketManager PacketManager;

        public Encryption Encryption;

		GrpcChannel _masterChannel;

		readonly double TIMEOUT_SECONDS = 99999.0;
        public static readonly UInt16 MAX_C2S_PACKET_LEN = 4096;

        DateTime timeConnected;

        private bool _busy = false;

        internal bool Dropped { get; private set; } = false;

        public Client(TcpClient tcpClient, XorKeyTable xorKeyTable, GrpcChannel masterChannel)
        {
            TcpClient = tcpClient;
            PacketManager = new PacketManager();
            Encryption = new(xorKeyTable);
			_masterChannel = masterChannel;

			var remoteEndPoint = TcpClient.Client.RemoteEndPoint as IPEndPoint;
            Ip = remoteEndPoint.Address.GetAddressBytes();
        }

        internal void OnConnect(UInt16 userIndex)
        {
            timeConnected = DateTime.UtcNow;
			//UInt32 unixTime = (UInt32)((DateTimeOffset)timeConnected).ToUnixTimeSeconds();
			var authKeyA = (UInt32)RandomNumberGenerator.GetInt32(Int32.MaxValue);
			var authKeyB = (UInt32)RandomNumberGenerator.GetInt32(Int32.MaxValue);
			ClientInfo = new(userIndex, authKeyA+authKeyB);
        }

		internal async Task<LoginAccountReply> SendLoginRequest(string username, string password)
		{
			var client = new AuthManager.AuthManagerClient(_masterChannel);
			var reply = await client.LoginAsync(new LoginAccountRequest { Username = username, Password = password });
			return reply;
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

                        if (packetLen < PacketC2S.HEADER_SIZE || packetLen > MAX_C2S_PACKET_LEN)
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
            }
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

					stream.Write(send);
					//PrintByteArray(send, send.Length, "encrypted sent");
				}
			}
            

        }

        internal async void Update(AccountManager accountManager)
        {
            if (_busy || Dropped)
                return;

            var actions = PacketManager.ReceiveAll();

			if(actions != null)
			{
				while(actions.Count > 0)
				{
					var action = actions.Dequeue();
					action(this);
				}
			}

            /*
			if(_clientInput.InputRegister != null)
			{
				_busy = true;
				var code = await accountManager.RequestRegister(_clientInput.InputRegister);
				_busy = false;
				// send the code..
				_packetManager.Send(new OutInfo((byte)code));
				Disconnect("registration completed");
				return;
			}
			else if (_clientInput.InputLogin != null)
			{
				_busy = true;
				var token = await accountManager.RequestLogin(_clientInput.InputLogin, DateTime.UtcNow.Ticks, Ip, Server.LOGIN_SECRET);
				_busy = false;
				if(token != null)
				{
					_packetManager.Send(new OutInfo((byte)InfoCode.LOGIN_SUCCESS));
					// send token
					Disconnect("login success");
				}
				else
				{
					_packetManager.Send(new OutInfo((byte)InfoCode.LOGIN_FAILED));
					Disconnect("login failed");
				}
				return;
			}
			*/
            //if (_clientInput.ReqConnect)
            //{

            //}
            //else

            var time = DateTime.UtcNow;

            if (time.Ticks - timeConnected.Ticks >= TimeSpan.FromSeconds(TIMEOUT_SECONDS).Ticks)
            {
                //timeout
                Disconnect("timeout");
                return;
            }
        }

        internal void Disconnect(string reason)
        {
            Dropped = true;
            //todo - send session timeout
        }

		internal async Task<ServerStateReply> GetServerState()
		{
			var client = new ChannelServiceAbc.ChannelServiceAbcClient(_masterChannel);
			var reply = await client.GetServerStateAsync(new ServerStateRequest{ Reserved = 0 });
			return reply;
		}
	}
}
