using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class IngameConnection
	{
		internal static void OnBackToCharLobby(Client client)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			if (client.ConnectionInfo.RequestedBackToCharLobby == true)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "already requested back to lobby");
				return;
			}

			var packet = new RSP_BackToCharLobby(1);
			client.PacketManager.Send(packet);

			client.ConnectionInfo.RequestedBackToCharLobby = true;
		}

		internal static void OnUninitialize(Client client, UInt16 index, Byte mapId, Byte option)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "Null Character");
				return;
			}

			if (option == 2 && client.ConnectionInfo.RequestedBackToCharLobby)
			{
				client.ConnectionInfo.RequestedBackToCharLobby = false;
				if (client.Character.Location.Instance.Type == Enums.InstanceType.FIELD)
				{
					client.Character.Location.DisconnectFieldInfo.MapId = (Int32)client.Character.Location.Instance.MapId;
					client.Character.Location.DisconnectFieldInfo.X = client.Character.Location.Movement.X;
					client.Character.Location.DisconnectFieldInfo.Y = client.Character.Location.Movement.Y;
				}
				else
				{
					client.Character.Location.DisconnectFieldInfo.MapId = client.Character.Location.LastFieldLocInfo.MapId;
					client.Character.Location.DisconnectFieldInfo.X = client.Character.Location.LastFieldLocInfo.X;
					client.Character.Location.DisconnectFieldInfo.Y = client.Character.Location.LastFieldLocInfo.Y;
				}
					
				client.Character.Location.Instance.RemoveClient(client, Enums.DelObjectType.LOGOUT);
				client.Character.Sync(Enums.DBSyncPriority.HIGH, true);
			}
			else
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "Unimplemented option");
				return;
			}
		}
	}
}
