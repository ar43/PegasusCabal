using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class CharSelect
	{
		internal static void OnCharacterRequest(Client client)
		{
			var packet = new RSP_GetMyChartr();
			client.PacketManager.Send(packet);
		}

		internal static void OnGetServerEnv(Client client)
		{
			var packet = new RSP_ServerEnv(ServerConfig.Get().GameSettings);
			client.PacketManager.Send(packet);
		}

		internal static void OnGetSvrTime(Client client)
		{
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalSeconds / 60;

			var packet = new RSP_GetSvrTime(time, (short)offset);
			client.PacketManager.Send(packet);
		}

		internal static void OnChargeInfoRequest(Client client)
		{
			var packet = new RSP_ChargeInfo(0, 0, 0);
			client.PacketManager.Send(packet);
		}
	}
}
