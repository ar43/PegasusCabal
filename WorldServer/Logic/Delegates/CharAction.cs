using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.CharData;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class CharAction
	{
		internal static void OnChangeStyle(Client client, Style newStyle, LiveStyle newLiveStyle, BuffFlag newBuffFlag, ActionFlag newActionFlag)
		{
			if(client.Character == null)
			{
				client.Disconnect("character not yet loaded", Enums.ConnState.ERROR);
				return;
			}

			byte toggleHelmet = newStyle.ShowHelmet;
			client.Character.Style.ToggleHelmet(toggleHelmet);

			//TODO: Verification
			client.Character.LiveStyle.Set(newLiveStyle.Serialize());
			client.Character.BuffFlag.Set(newBuffFlag.Serialize());
			client.Character.ActionFlag.Set(newActionFlag.Serialize());

			var packet_rsp = new RSP_ChangeStyle(1); //TODO: send 0 if bad
			client.PacketManager.Send(packet_rsp);

			var packet_nfy = new NFY_ChangeStyle(client.Character);
			client.BroadcastAround(packet_nfy);
		}
	}
}
