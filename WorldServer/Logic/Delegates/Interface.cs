using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Interface
	{
		internal static void OnUpdateHelpInfo(Client client)
		{
			var packet = new RSP_UpdateHelpInfo(1);
			client.PacketManager.Send(packet);
		}
	}
}
