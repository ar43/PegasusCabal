using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Delegates
{
	internal static class Warping
	{
		internal static void OnWarpCommand(Client client, Byte warpCommandId, UInt16 slot, UInt32 worldType, UInt32 target)
		{
			var character = client.Character;
			var instance = client.Character.Location.Instance;
			if(character == null || instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character or instance is null");
				return;
			}

			switch(warpCommandId)
			{
				case 62:
				{
					client.World.InstanceManager.WarpClientReturn(client);
					break;
				}
				default:
				{
					client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "invalid warpCommandId");
					return;
				}
			}
		}
	}
}
