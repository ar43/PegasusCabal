using LibPegasus.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.Extra;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class WorldChat
	{
		internal static void OnLocalMessageRequest(Client client, MsgType msgType, String message)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			if(msgType != MsgType.Normal)
				throw new NotImplementedException();

			if(message.StartsWith('!'))
			{
				var cmdName = message.Split(' ')[0].Substring(1);

				if(CommandManager.CommandList.TryGetValue(cmdName.ToLower(), out var cmd))
				{
					cmd(client, Utility.GetCmdArgs(message));
					return;
				}
			}

			var packet_response = new NFY_MessageEvnt(client.Character.Id, msgType, message);
			client.BroadcastNearby(packet_response);
		}
	}
}
