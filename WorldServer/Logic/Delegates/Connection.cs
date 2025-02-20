﻿using LibPegasus.Crypt;
using LibPegasus.Enums;
using WorldServer.Enums;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Connection
	{
		internal static void OnServerConnection(Client client, Byte serverId, Byte channelId)
		{
			var cfg = ServerConfig.Get();
			if (client.ConnectionInfo.ConnState != ConnState.UNCONNECTED)
			{
				client.Disconnect("invalid handshake", ConnState.ERROR);
				return;
			}

			if (serverId != cfg.GeneralSettings.ServerId || channelId != cfg.GeneralSettings.ChannelId)
			{
				client.Disconnect("invalid handshake (bad serverId or clientId)", ConnState.ERROR);
				return;
			}

			client.ConnectionInfo.ConnState = ConnState.AWAITING;

			var packet = new RSP_Connect2Svr(Encryption.Recv2ndXorSeed, client.ConnectionInfo.AuthKey, client.ConnectionInfo.UserId, client.Encryption.RecvXorKeyIdx);
			client.PacketManager.Send(packet);
		}

		internal async static void OnVerifyLinks(Client client, UInt32 authKey, UInt16 userId, Byte channelId, Byte serverId, UInt32 clientMagicKey)
		{
			var cfg = ServerConfig.Get();
			//TODO: FIX THIS! STUDY HOW THIS WORKS! IT CHANGES!
			//if (clientMagicKey != cfg.GeneralSettings.ClientMagicKey)
			//{
			//	//TODO: Close connection
			//	throw new NotImplementedException();
			//}

			client.ConnectionInfo.ConnState = ConnState.AWAITING_LINK_REPLY;
			//TODO: check if authKey expired (5 sec?)
			var reply = await client.SendLoginSessionRequest(authKey, userId, channelId, serverId);
			bool success = reply.Result == (uint)SessionResult.OK || reply.Result == (uint)SessionResult.REPLACED;
			var packet = new RSP_VerifyLinks(channelId, serverId, success);
			client.PacketManager.Send(packet);

			if (success)
			{
				//client.ClientInfo.ConnState = Enums.ConnState.VERIFIED;
				client.Disconnect("Linked - success", ConnState.LINK_EXIT);

				//TODO: disconnect??
			}
			else
			{
				client.Disconnect("Linked - fail", ConnState.ERROR);
			}
		}
	}
}
