using LibPegasus.Packets;
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
		public Instance(UInt16 worldId, InstanceType type)
		{
			WorldId = (Enums.WorldId)worldId;
			_tiles = new Tile[16, 16];
			Type = type;

			for(int i = 0; i < _tiles.GetLength(0); i++)
			{
				for(int j = 0; j < _tiles.GetLength(1); j++)
				{
					_tiles[i, j] = new Tile();
				}
			}

			if (type == InstanceType.PERMANENT)
			{
				Id = (UInt64)WorldId;
			}
			else
			{
				Id = InstanceIdGenerator;
				InstanceIdGenerator++;
			}
		}

		public Instance(Enums.WorldId worldId, InstanceType type) : this((UInt16)worldId, type) { }

		private readonly Tile[,] _tiles;
		public UInt128 Id { get;}
		public Enums.WorldId WorldId { get; }
		public InstanceType Type { get;}

		private static UInt128 InstanceIdGenerator = 1000;

		public void AddNewClient(Client client, UInt16 tileX, UInt16 tileY)
		{
			_tiles[tileY, tileX].localClients.Add(client);
		}

		public void RemoveClient(Client client, DelUserType reason)
		{
			var packet_del = new NFY_DelUserList(client.Character.Id, reason);
			BroadcastNearby(client, packet_del, true);

			foreach(var tile in _tiles)
			{
                if (tile.localClients.Remove(client))
                {
					return;
                }
			}
		}
		public void MoveClient(Client client, UInt16 newTileX, UInt16 newTileY, NewUserType tileMoveType)
		{
			var tileX = client.Character.Location.TileX; //old tile pos x
			var tileY = client.Character.Location.TileY; //old tile pos y

			var currentTile = _tiles[tileY, tileX];
			var newTile = _tiles[newTileY, newTileX];

			//TODO verify difference between current and new Tile

			var currentNeighbours = GetNeighbours((Int16)tileX, (Int16)tileY);
			var newNeightbours = GetNeighbours((Int16)newTileX, (Int16)newTileY);

			var relevantTiles = newNeightbours.Except(currentNeighbours).ToList();

			List<Character> newChars = new List<Character>();

			foreach (var tilePos in relevantTiles)
			{
				foreach (var c in _tiles[tilePos.Item2, tilePos.Item1].localClients)
				{
					if (c == client)
						continue;
					if (c.Character == null)
						throw new Exception("Character should not be null here");
					if (client.Character == null)
						throw new Exception("Character should not be null here");

					newChars.Add(c.Character);
					var packetNewUser = new NFY_NewUserList(new List<Character>() { client.Character }, tileMoveType);
					c.PacketManager.Send(packetNewUser);
				}
			}

			if(newChars.Count > 0)
			{
				var packetOtherPlayers = new NFY_NewUserList(newChars, NewUserType.OTHERPLAYERS);
				client.PacketManager.Send(packetOtherPlayers);
			}

			currentTile.localClients.Remove(client);
			newTile.localClients.Add(client);

			client.Character.Location.UpdateTilePos();
		}
		private static List<(Int16, Int16)> GetNeighbours(Int16 tileX, Int16 tileY)
		{
			List<(Int16, Int16)> values = new List<(Int16, Int16)>();

			Int16[,] offsets = { { 0, 0 }, { 1, 0 }, { 2, 0 }, { -1, 0 }, { -2, 0 },
										   { 0, 1 }, { 0, 2 }, { 0, -1 }, { 0, -2 },
								{ 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };

			for(int i = 0; i < 13; i++)
			{
				var x = tileX + offsets[i, 0];
				var y = tileY + offsets[i, 1];
				if(x >= 0 && y >= 0)
				{
					values.Add(((Int16, Int16))(x, y));
				}
			}
			return values;
		}

		public void BroadcastNearby(Client client, PacketS2C packet, bool excludeClient)
		{
			var tileX = client.Character.Location.TileX;
			var tileY = client.Character.Location.TileY;

			foreach(var tilePos in GetNeighbours((Int16)tileX, (Int16)tileY))
			{
				foreach(var c in _tiles[tilePos.Item2, tilePos.Item1].localClients)
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

			var tileX = client.Character.Location.TileX;
			var tileY = client.Character.Location.TileY;

			foreach (var tilePos in GetNeighbours((Int16)tileX, (Int16)tileY))
			{
				foreach (var c in _tiles[tilePos.Item2, tilePos.Item1].localClients)
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
