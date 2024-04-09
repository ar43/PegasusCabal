using LoginServer.Opcodes.S2C;
using LoginServer.Packets.S2C;
using LoginServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Logic.Delegates
{
	internal static class Connection
	{
		public static void OnServerConnection(Client client)
		{
			Serilog.Log.Debug("OnServerConnection called");

			if(client.ClientInfo.ConnState != Enums.ConnState.INITIAL)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			client.ClientInfo.ConnState = Enums.ConnState.CONNECTED;

			var packet = new RSP_Connect2Serv(Crypt.Encryption.Recv2ndXorSeed, client.ClientInfo.AuthKey, client.ClientInfo.UserId, client.Encryption.RecvXorKeyIdx);
			client.PacketManager.Send(packet);
		}

		public static void OnCheckVersion(Client client, UInt32 clientVersion)
		{
			var serverConfig = ServerConfig.Get();

			if (client.ClientInfo.ConnState != Enums.ConnState.CONNECTED)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}

			if (serverConfig.GeneralSettings.VerifyClientVersion)
			{
				if(clientVersion != (uint)serverConfig.GeneralSettings.ClientVersion)
				{
					//TODO: check what happens if clientVersion is bogus
					throw new NotImplementedException();
				}
			}

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

			if(username.Length < 1 || username.Length > 16)
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

		public static void OnAuthAccount(Client client, byte[] rsaData)
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
			var username = System.Text.ASCIIEncoding.ASCII.GetString(decryptedRSA, 0, usernameLen);

			var passwordLen = Array.IndexOf(decryptedRSA, (byte)0, 33) - 33;
			if (passwordLen <= 0)
			{
				//TODO: Close connection
				throw new NotImplementedException();
			}
			var password = System.Text.ASCIIEncoding.ASCII.GetString(decryptedRSA, 33, passwordLen);

			Serilog.Log.Debug($"username extracted: {username} (len: {username.Length})");
			Serilog.Log.Debug($"password extracted: {password} (len: {password.Length})");

			client.ClientInfo.ConnState = Enums.ConnState.AUTH_ACCOUNT;


		}

	}
}
