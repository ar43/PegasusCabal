using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Delegates
{
	internal static class Miscellaneous
	{
		internal static void OnHeartbeatResponse(Client client)
		{
			client.TimerHeartbeatTimeout = null;
		}
	}
}
