using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Movement
	{
		internal static void OnMoveBegin(Client client, UInt16 fromX, UInt16 fromY, UInt16 toX, UInt16 toY, UInt16 pntX, UInt16 pntY, UInt16 worldId)
		{
			//TODO: verify
			if (client.Character == null)
			{
				client.Disconnect("OnMoveBegin null", Enums.ConnState.ERROR);
				return;
			}

			var loc = client.Character.Location;

			if (loc.Instance == null)
			{
				client.Disconnect("OnMoveBegin null", Enums.ConnState.ERROR);
				return;
			}

			//terrible
			loc.X = fromX;
			loc.Y = fromY;

			loc.Movement.Start(fromX, fromY, toX, toY);
			long unixTime = ((DateTimeOffset)loc.Movement.StartTime).ToUnixTimeSeconds();

			var packet = new NFY_MoveBegined((UInt32)client.Character.Id, (UInt32)unixTime, fromX, fromY, toX, toY);
			client.BroadcastNearby(packet);
		}

		internal static void OnMoveEnd(Client client, UInt16 x, UInt16 y)
		{
			//TODO: verify
			if (client.Character == null)
			{
				client.Disconnect("OnMoveEnd null", Enums.ConnState.ERROR);
				return;
			}

			var loc = client.Character.Location;

			if (loc.Instance == null)
			{
				client.Disconnect("OnMoveEnd null", Enums.ConnState.ERROR);
				return;
			}

			//terrible
			loc.X = x;
			loc.Y = y;

			loc.Movement.End();

			var packet = new NFY_MoveEnded00((UInt32)client.Character.Id, x,y);
			client.BroadcastNearby(packet);

		}

		internal static void OnTileChange(Client client, UInt16 x, UInt16 y)
		{
			//TODO: verify
			if(client.Character == null)
			{
				client.Disconnect("OnTileChange null", Enums.ConnState.ERROR);
				return;
			}
			var loc = client.Character.Location;

			if(loc.Instance == null)
			{
				client.Disconnect("OnTileChange null", Enums.ConnState.ERROR);
				return;
			}

			loc.X = x; 
			loc.Y = y;

			loc.Instance.MoveClient(client, (UInt16)(x / 16), (UInt16)(y / 16), Enums.NewUserType.NEWMOVE);

			loc.UpdateTilePos();
		}
	}
}
