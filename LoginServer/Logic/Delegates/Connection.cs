using LibPegasus.Crypt;
using LibPegasus.Enums;
using LibPegasus.Utils;
using LoginServer.Packets.S2C;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace LoginServer.Logic.Delegates
{
	internal static class Connection
	{
		public static void OnServerConnection(Client client)
		{
			Serilog.Log.Debug("OnServerConnection called");

			if (client.ClientInfo.ConnState != Enums.ConnState.INITIAL)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			client.ClientInfo.ConnState = Enums.ConnState.CONNECTED;

			var packet = new RSP_Connect2Svr(Encryption.Recv2ndXorSeed, client.ClientInfo.AuthKey, client.ClientInfo.UserId, client.Encryption.RecvXorKeyIdx);
			client.PacketManager.Send(packet);
		}

		public static void OnCheckVersion(Client client, UInt32 clientVersion)
		{
			var serverConfig = ServerConfig.Get();

			if (client.ClientInfo.ConnState != Enums.ConnState.CONNECTED && client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			if (serverConfig.GeneralSettings.VerifyClientVersion)
			{
				if (clientVersion != (uint)serverConfig.GeneralSettings.ClientVersion)
				{
					//TODO: check what happens if clientVersion is bogus
					throw new NotImplementedException();
				}
			}

			if (client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT)
				client.ClientInfo.ConnState = Enums.ConnState.VERSION_CHECKED;

			var packet = new RSP_CheckVersion((uint)serverConfig.GeneralSettings.ClientVersion);
			client.PacketManager.Send(packet);
		}

		public static void OnPreServerEnvRequest(Client client, string username)
		{
			if (client.ClientInfo.ConnState != Enums.ConnState.VERSION_CHECKED)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			if (username.Length < 1 || username.Length > 16)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			//client.ClientInfo.Username = username;
			client.ClientInfo.ConnState = Enums.ConnState.PRE_ENV;

			var packet = new RSP_PreServerEnvRequest();
			client.PacketManager.Send(packet);
		}

		public static void OnPublicKeyRequest(Client client)
		{
			if (client.ClientInfo.ConnState != Enums.ConnState.PRE_ENV)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			var publicKey = client.ClientInfo.RSA.ExportRSAPublicKey();
			Serilog.Log.Debug($"publicKey with len {publicKey.Length} copied");
			client.ClientInfo.ConnState = Enums.ConnState.PUBLIC_KEY_REQUESTED;

			var packet = new RSP_PublicKey(publicKey);
			client.PacketManager.Send(packet);
		}

		public static async void OnAuthAccount(Client client, byte[] rsaData)
		{
			if (client.ClientInfo.ConnState != Enums.ConnState.PUBLIC_KEY_REQUESTED)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			var decryptedRSA = client.ClientInfo.RSA.Decrypt(rsaData, RSAEncryptionPadding.OaepSHA1);

			Utility.PrintCharArray(decryptedRSA, decryptedRSA.Length, "rsa output");

			var usernameLen = Array.IndexOf(decryptedRSA, (byte)0, 0, 33);
			if (usernameLen <= 0)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}
			var username = Encoding.ASCII.GetString(decryptedRSA, 0, usernameLen);

			var passwordLen = Array.IndexOf(decryptedRSA, (byte)0, 33) - 33;
			if (passwordLen <= 0)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}
			var password = Encoding.ASCII.GetString(decryptedRSA, 33, passwordLen);

			//Serilog.Log.Debug($"username extracted: {username} (len: {username.Length})");
			//Serilog.Log.Debug($"password extracted: {password} (len: {password.Length})");

			var reply = await client.SendLoginRequest(username, password);
			if ((AuthResult)reply.Status == AuthResult.Normal)
			{
				bool isLocalhost = client.Ip == "127.0.0.1";
				Debug.Assert(reply.AuthKey.Length == 32);
				var replyServerState = await client.GetServerState(isLocalhost);

				var packetServerState = new NFY_ServerState(replyServerState);
				client.PacketManager.Send(packetServerState);

				var packetUrlToClient = new NFY_UrlToClient();
				client.PacketManager.Send(packetUrlToClient);

				var packetAuth = new RSP_AuthAccount(reply);
				client.PacketManager.Send(packetAuth);

				var packetMsg = new NFY_SystemMessg(Enums.MessageType.Normal2, "");
				client.PacketManager.Send(packetMsg);
				client.ClientInfo.ConnState = Enums.ConnState.AUTH_ACCOUNT;
				client.ClientInfo.AccountId = reply.AccountId;

			}
			else
			{
				var packet = new RSP_AuthAccount(reply);
				client.PacketManager.Send(packet);
				client.Disconnect("bad auth");
			}
		}

		internal static async void OnVerifyLinks(Client client, UInt32 authKey, UInt16 userId, Byte channelId, Byte serverId, UInt32 clientMagicKey)
		{
			var cfg = ServerConfig.Get();
			if (client.ClientInfo.ConnState != Enums.ConnState.AUTH_ACCOUNT || client.ClientInfo.AccountId == 0)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			if(clientMagicKey != cfg.GeneralSettings.ClientMagicKey)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}
			client.ClientInfo.ConnState = Enums.ConnState.VERIFYING;
			//TODO: check if authKey expired (5 sec?)
			var reply = await client.SendSessionRequest(authKey, userId, channelId, serverId);
			bool success = reply.Result == (uint)SessionResult.OK || reply.Result == (uint)SessionResult.REPLACED;
			var packet = new RSP_VerifyLinks(channelId, serverId, success);
			client.PacketManager.Send(packet);

			if (success)
			{
				client.ClientInfo.ConnState = Enums.ConnState.VERIFIED;
				client.Disconnect("Linked - success");

				//TODO: disconnect??
			}
			else
			{
				client.Disconnect("Linked - fail");
			}
			
		}
	}
}
