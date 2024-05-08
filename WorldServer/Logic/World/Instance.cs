using LibPegasus.Packets;
using System;
using System.Collections.Generic;
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

		private Tile[,] _tiles = new Tile[16, 16];
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
		private static List<(Int16, Int16)> GetNeighbours(Int16 tileX, Int16 tileY)
		{
			List<(Int16, Int16)> values = new List<(Int16, Int16)>();

			Int16[,] offsets = { { 0, 0 }, { 1, 0 }, { 2, 0 }, { -1, 0 }, { -2, 0 },
										   { 0, 1 }, { 0, 2 }, { 0, -1 }, { 0, -2 },
								{ 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };

			for(int i = 0; i < 13; i++)
			{
				var x = tileX + offsets[i, 0];
				var y = tileX + offsets[i, 1];
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
