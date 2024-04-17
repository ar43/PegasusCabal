using LibPegasus.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Connection
	{
		internal static void OnServerConnection(Client client, Byte serverId, Byte channelId)
		{
			var cfg = ServerConfig.Get();
			if(client.ConnectionInfo.ConnState != ConnState.UNCONNECTED)
			{
				client.Disconnect("invalid handshake", ConnState.ERROR);
				return;
			}

			if(serverId != cfg.GeneralSettings.ServerId || channelId != cfg.GeneralSettings.ChannelId)
			{
				client.Disconnect("invalid handshake (bad serverId or clientId)", ConnState.ERROR);
				return;
			}

			client.ConnectionInfo.ConnState = ConnState.AWAITING;

			var packet = new RSP_Connect2Svr(Encryption.Recv2ndXorSeed, client.ConnectionInfo.AuthKey, client.ConnectionInfo.UserId, client.Encryption.RecvXorKeyIdx);
			client.PacketManager.Send(packet);
		}
	}
}
