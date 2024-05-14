using LibPegasus.Packets;
using LibPegasus.Parsers.Mcl;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.World
{
	internal class Instance
	{
		public Instance(UInt16 mapId, InstanceType type)
		{
			MapId = (Enums.MapId)mapId;
			_cells = new Cell[NUM_CELL_X, NUM_CELL_Y];
			Type = type;

			for(int i = 0; i < _cells.GetLength(0); i++)
			{
				for(int j = 0; j < _cells.GetLength(1); j++)
				{
					_cells[i, j] = new Cell();
				}
			}

			if (type == InstanceType.PERMANENT)
			{
				Id = (UInt64)MapId;
			}
			else
			{
				Id = InstanceIdGenerator;
				InstanceIdGenerator++;
			}
		}

		public Instance(Enums.MapId worldId, InstanceType type) : this((UInt16)worldId, type) { }

		private readonly Cell[,] _cells;
		public TileAttributeData? TileAttributeData { get; set; }
		public UInt128 Id { get;}
		public Enums.MapId MapId { get; }
		public InstanceType Type { get;}

		private static UInt128 InstanceIdGenerator = 1000;

		public static readonly int NUM_CELL_X = 16;
		public static readonly int NUM_CELL_Y = 16;

		public void AddNewClient(Client client, UInt16 cellX, UInt16 cellY)
		{
			_cells[cellX, cellY].LocalClients.Add(client);
		}

		public void RemoveClient(Client client, DelUserType reason)
		{
			var packet_del = new NFY_DelUserList(client.Character.Id, reason);
			BroadcastNearby(client, packet_del, true);

			foreach(var cell in _cells)
			{
                if (cell.LocalClients.Remove(client))
                {
					return;
                }
			}
		}
		public bool CheckTerrainCollision(UInt16 x, UInt16 y)
		{
			return TileAttributeData.HasTileAttribute(x, y, LibPegasus.Enums.TileAttribute.WALL);
		}
		public void MoveClient(Client client, UInt16 newCellX, UInt16 newCellY, NewUserType cellMoveType)
		{
			var cellX = client.Character.Location.Movement.CellX; //old cell pos x
			var cellY = client.Character.Location.Movement.CellY; //old cell pos y

			var currentCell = _cells[cellX, cellY];
			var newCell = _cells[newCellX, newCellY];

			//TODO verify difference between current and new Cell

			var currentNeighbours = GetNeighbours((Int16)cellX, (Int16)cellY);
			var newNeightbours = GetNeighbours((Int16)newCellX, (Int16)newCellY);

			var relevantCells = newNeightbours.Except(currentNeighbours).ToList();

			List<Character> newChars = new List<Character>();

			foreach (var cellPos in relevantCells)
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == client)
						continue;
					if (c.Character == null)
						throw new Exception("Character should not be null here");
					if (client.Character == null)
						throw new Exception("Character should not be null here");

					newChars.Add(c.Character);
					var packetNewUser = new NFY_NewUserList(new List<Character>() { client.Character }, cellMoveType);
					c.PacketManager.Send(packetNewUser);
				}
			}

			if(newChars.Count > 0)
			{
				var packetOtherPlayers = new NFY_NewUserList(newChars, NewUserType.OTHERPLAYERS);
				client.PacketManager.Send(packetOtherPlayers);
			}

			currentCell.LocalClients.Remove(client);
			newCell.LocalClients.Add(client);

			client.Character.Location.Movement.UpdateCellPos();
		}
		private static List<(Int16, Int16)> GetNeighbours(Int16 cellX, Int16 cellY)
		{
			List<(Int16, Int16)> values = new List<(Int16, Int16)>();

			Int16[,] offsets = { { 0, 0 }, { 1, 0 }, { 2, 0 }, { -1, 0 }, { -2, 0 },
										   { 0, 1 }, { 0, 2 }, { 0, -1 }, { 0, -2 },
								{ 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };

			for(int i = 0; i < 13; i++)
			{
				var x = cellX + offsets[i, 0];
				var y = cellY + offsets[i, 1];
				if(x >= 0 && y >= 0 && x < NUM_CELL_X && y < NUM_CELL_Y)
				{
					values.Add(((Int16, Int16))(x, y));
				}
			}
			return values;
		}

		public void BroadcastNearby(Client client, PacketS2C packet, bool excludeClient)
		{
			var cellX = client.Character.Location.Movement.CellX;
			var cellY = client.Character.Location.Movement.CellY;

			foreach(var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach(var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == null)
						throw new NullReferenceException("null client");
					if (c.Character == null)
						throw new NullReferenceException("null client");
					if (c == client && excludeClient)
						continue;

					c.PacketManager.Send(packet);
				}
			}
		}
		public List<Character> GetNearbyCharacters(Client client)
		{
			List<Character> characters = new List<Character>();

			var cellX = client.Character.Location.Movement.CellX;
			var cellY = client.Character.Location.Movement.CellY;

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == client)
						continue;

					if(c.Character == null)
						throw new NullReferenceException("null character");

					characters.Add(c.Character);
				}
			}
			return characters;
		}



	}
}
