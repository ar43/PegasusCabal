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

			if (loc.Movement.IsMoving == true)
			{
				client.Disconnect("OnMoveBegin: Already moving", Enums.ConnState.ERROR);
				return;
			}

			if(loc.Movement.Begin(fromX, fromY, toX, toY, pntX, pntY))
			{
				var packet = new NFY_MoveBegined((UInt32)client.Character.Id, (UInt32)loc.Movement.StartTime, fromX, fromY, toX, toY);
				client.BroadcastNearby(packet);
			}
			else
			{
				client.Disconnect("Error in Movement.Begin", Enums.ConnState.ERROR);
				return;
			}

			
		}

		internal static void OnMoveChanged(Client client, UInt16 fromX, UInt16 fromY, UInt16 toX, UInt16 toY, UInt16 pntX, UInt16 pntY, UInt16 worldId)
		{
			//TODO: verify
			if (client.Character == null)
			{
				client.Disconnect("OnMoveChanged null", Enums.ConnState.ERROR);
				return;
			}

			var loc = client.Character.Location;

			if (loc.Instance == null)
			{
				client.Disconnect("OnMoveChanged null", Enums.ConnState.ERROR);
				return;
			}

			if(loc.Movement.IsMoving == false)
			{
				client.Disconnect("OnMoveChanged: Wasn't moving", Enums.ConnState.ERROR);
				return;
			}

			if(fromX >= 256 || fromY >= 256 || toX >= 256 || toY >= 256 || pntX >= 256 || pntY >= 256)
			{
				client.Disconnect("OnMoveChanged: Invalid input", Enums.ConnState.ERROR);
				return;
			}

			if(loc.Movement.Change(fromX, fromY, toX,toY,pntX,pntY))
			{
				var packet = new NFY_MoveChanged((UInt32)client.Character.Id, (UInt32)loc.Movement.StartTime, fromX, fromY, toX, toY);
				client.BroadcastNearby(packet);
			}
			else
			{
				client.Disconnect("Error in Movement.Change", Enums.ConnState.ERROR);
				return;
			}
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

			if (loc.Movement.IsMoving == false)
			{
				client.Disconnect("OnMoveEnd: Wasn't moving", Enums.ConnState.ERROR);
				return;
			}

			if(loc.Movement.End(x, y))
			{
				var packet = new NFY_MoveEnded00((UInt32)client.Character.Id, x, y);
				client.BroadcastNearby(packet);
			}
			else
			{
				client.Disconnect("Error in Movement.End", Enums.ConnState.ERROR);
				return;
			}
		}

		internal static void OnMoveWaypoint(Client client, UInt16 fromX, UInt16 fromY, UInt16 toX, UInt16 toY)
		{
			//TODO: verify
			if (client.Character == null)
			{
				client.Disconnect("OnMoveWaypoint null", Enums.ConnState.ERROR);
				return;
			}
			var loc = client.Character.Location;

			if (loc.Instance == null)
			{
				client.Disconnect("OnMoveWaypoint null", Enums.ConnState.ERROR);
				return;
			}

			if (loc.Movement.IsMoving == false)
			{
				client.Disconnect("OnMoveWaypoint: Wasn't moving", Enums.ConnState.ERROR);
				return;
			}

			if(!loc.Movement.SwitchWaypoint(fromX, fromY, toX, toY))
			{
				client.Disconnect("Movement.SwitchWaypoint: error", Enums.ConnState.ERROR);
				return;
			}
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

			if (loc.Movement.IsMoving == false)
			{
				client.Disconnect("OnTileChange: Wasn't moving", Enums.ConnState.ERROR);
				return;
			}

			if (loc.Movement.TileMove(x, y))
			{
				loc.Instance.MoveClient(client, (UInt16)(x / 16), (UInt16)(y / 16), Enums.NewUserType.NEWMOVE);
				loc.Movement.UpdateTilePos();
			}
			else
			{
				client.Disconnect("OnTileChange: fail", Enums.ConnState.ERROR);
				return;
			}
			

			

			
		}
	}
}
