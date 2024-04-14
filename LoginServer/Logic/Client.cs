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
using System.Net;
using LibPegasus.Enums;
using LibPegasus.Crypt;
using LibPegasus.Packets;
using System.Net.Security;
using System.Security.Cryptography;
using LibPegasus.Utils;
using Grpc.Net.Client;
using Shared.Protos;
using LoginServer.Packets;

namespace LoginServer.Logic
{
    internal class Client
    {
        public TcpClient TcpClient { private set; get; }

		public ClientInfo? ClientInfo { private set; get; }

        internal string Ip { private set; get; }

        public PacketManager PacketManager;

        public Encryption Encryption;

		GrpcChannel _masterRpcChannel;

		readonly double TIMEOUT_SECONDS = 99999.0;

        DateTime timeConnected;

        private bool _busy = false;

        internal bool Dropped { get; private set; } = false;

        public Client(TcpClient tcpClient, XorKeyTable xorKeyTable, GrpcChannel masterChannel)
        {
            TcpClient = tcpClient;
            PacketManager = new PacketManager();
            Encryption = new(xorKeyTable);
			_masterRpcChannel = masterChannel;

			var remoteEndPoint = TcpClient.Client.RemoteEndPoint as IPEndPoint;
            Ip = remoteEndPoint.Address.ToString();
			Log.Debug(Ip);
        }

        internal void OnConnect(UInt16 userIndex)
        {
            timeConnected = DateTime.UtcNow;
			UInt32 unixTime = (UInt32)((DateTimeOffset)timeConnected).ToUnixTimeSeconds();
			ClientInfo = new(userIndex, unixTime);
        }

		internal async Task<LoginAccountReply> SendLoginRequest(string username, string password)
		{
			var client = new AuthMaster.AuthMasterClient(_masterRpcChannel);
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

		internal async Task<ServerStateReply> GetServerState(bool isLocalhost)
		{
			var client = new ChannelMaster.ChannelMasterClient(_masterRpcChannel);
			var reply = await client.GetServerStateAsync(new ServerStateRequest{ IsLocalhost = isLocalhost });
			return reply;
		}
	}
}
