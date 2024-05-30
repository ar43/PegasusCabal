using LibPegasus.Packets;
using LibPegasus.Parsers.Mcl;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime
{
    internal class Instance
	{
		public Instance(UInt16 mapId, InstanceType type, MapData mapData)
		{
			MapId = (MapId)mapId;
			_cells = new Cell[NUM_CELL_X, NUM_CELL_Y];
			MapData = mapData;
			Type = type;
			NumClients = 0;

			for (int i = 0; i < _cells.GetLength(0); i++)
			{
				for (int j = 0; j < _cells.GetLength(1); j++)
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

		public Instance(MapId mapId, InstanceType type, MapData mapData) : this((UInt16)mapId, type, mapData) { }

		private readonly Cell[,] _cells;
		public readonly MapData MapData;
		public MobManager? MobManager = null;

		public TileAttributeData? TileAttributeData { get; set; }
		public UInt64 Id { get; }
		public MapId MapId { get; }
		public InstanceType Type { get; }

		private static UInt64 InstanceIdGenerator = 1000;

		public static readonly int NUM_CELL_X = 16;
		public static readonly int NUM_CELL_Y = 16;

		public int NumClients { get; private set; }

		public void AddNewClient(Client client, UInt16 cellX, UInt16 cellY)
		{
			NumClients++;
			_cells[cellX, cellY].LocalClients.Add(client);
		}

		public void AddMobToCell(Mob mob, UInt16 cellX, UInt16 cellY, bool notifyAround)
		{
			_cells[cellX, cellY].LocalMobs.Add(mob);
			if (notifyAround)
			{
				//possibly optimize this... no need to broadcast to empty instance for example
				BroadcastNearby(mob, new NFY_NewMobsList(new List<Mob> { mob }));
			}
		}

		public void RemoveMobFromCell(Mob mob, bool notifyAround, DelObjectType type)
		{
			_cells[mob.Movement.CellX, mob.Movement.CellY].LocalMobs.Add(mob);
			if (notifyAround)
			{
				//possibly optimize this... no need to broadcast to empty instance for example
				BroadcastNearby(mob, new NFY_DelMobsList(mob.ObjectIndexData, type));
			}
		}

		public List<(int, int)> CalculateValidCellSpots(UInt16 cellX, UInt16 cellY, bool mustBeTown)
		{
			List<(int, int)> values = new List<(int, int)>();
			var baseX = cellX * 16;
			var baseY = cellY * 16;

			for (int i = 0; i < 16; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					if (!CheckTerrainCollision((UInt16)(baseX + i), (UInt16)(baseY + j)))
					{
						if (!mustBeTown || CheckTileTown((UInt16)(baseX + i), (UInt16)(baseY + j)))
							values.Add((baseX + i, baseY + j));
					}
				}
			}
			return values;
		}

		public void RemoveClient(Client client, DelObjectType reason)
		{
			var packet_del = new NFY_DelUserList(client.Character.Id, reason);
			BroadcastNearby(client, packet_del, false);

			foreach (var cell in _cells)
			{
				if (cell.LocalClients.Remove(client))
				{
					client.Character.Location.Instance = null;
					NumClients--;
					if (NumClients < 0)
						throw new Exception("negative NumClients");
					return;
				}
			}

			throw new Exception("not expected");
		}
		public bool CheckTerrainCollision(UInt16 x, UInt16 y)
		{
			if (x > 255 || y > 255)
				return true;
			return TileAttributeData.HasTileAttribute(x, y, LibPegasus.Enums.TileAttribute.WALL);
		}
		public bool CheckTileTown(UInt16 x, UInt16 y)
		{
			if (x > 255 || y > 255)
				return false;
			return TileAttributeData.HasTileAttribute(x, y, LibPegasus.Enums.TileAttribute.TOWN);
		}
		public void MoveMob(Mob mob, UInt16 newCellX, UInt16 newCellY)
		{
			var cellX = mob.Movement.CellX; //old cell pos x
			var cellY = mob.Movement.CellY; //old cell pos y

			if (Math.Abs(cellX - newCellX) > 1 || Math.Abs(cellY - newCellY) > 1)
				throw new Exception("incorrent MoveMob");

			var currentCell = _cells[cellX, cellY];
			var newCell = _cells[newCellX, newCellY];

			//TODO verify difference between current and new Cell

			var currentNeighbours = GetNeighbours((Int16)cellX, (Int16)cellY);
			var newNeightbours = GetNeighbours((Int16)newCellX, (Int16)newCellY);

			var relevantCells = newNeightbours.Except(currentNeighbours).ToList();

			foreach (var cellPos in relevantCells)
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c.Character == null)
						throw new Exception("Character should not be null here");

					var packetNewUser = new NFY_NewMobsList(new List<Mob>() { mob });
					c.PacketManager.Send(packetNewUser);
				}
			}

			currentCell.LocalMobs.Remove(mob);
			newCell.LocalMobs.Add(mob);

			mob.Movement.UpdateCellPos();
		}
		public void MoveClient(Client client, UInt16 newCellX, UInt16 newCellY, AddObjectType cellMoveType)
		{
			var cellX = client.Character.Location.Movement.CellX; //old cell pos x
			var cellY = client.Character.Location.Movement.CellY; //old cell pos y

			if (Math.Abs(cellX - newCellX) > 1 || Math.Abs(cellY - newCellY) > 1)
				throw new Exception("incorrent MoveClient");

			var currentCell = _cells[cellX, cellY];
			var newCell = _cells[newCellX, newCellY];

			//TODO verify difference between current and new Cell

			var currentNeighbours = GetNeighbours((Int16)cellX, (Int16)cellY);
			var newNeightbours = GetNeighbours((Int16)newCellX, (Int16)newCellY);

			var relevantCells = newNeightbours.Except(currentNeighbours).ToList();

			List<Character> newChars = new List<Character>();
			List<Mob> newMobs = new List<Mob>();

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

				foreach (var m in _cells[cellPos.Item1, cellPos.Item2].LocalMobs)
				{
					if (m == null)
						throw new NullReferenceException("null mob");
					if (m.IsSpawned == false)
						throw new Exception("mob not spawned, shouldnt be in a cell");

					newMobs.Add(m);
				}
			}

			if (newChars.Count > 0)
			{
				var packetOtherPlayers = new NFY_NewUserList(newChars, AddObjectType.OTHERPLAYERS);
				client.PacketManager.Send(packetOtherPlayers);
			}

			if (newMobs.Count > 0)
			{
				var packetNewMobs = new NFY_NewMobsList(newMobs);
				client.PacketManager.Send(packetNewMobs);
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

			for (int i = 0; i < 13; i++)
			{
				var x = cellX + offsets[i, 0];
				var y = cellY + offsets[i, 1];
				if (x >= 0 && y >= 0 && x < NUM_CELL_X && y < NUM_CELL_Y)
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

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
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
		public void BroadcastNearby(Mob mob, PacketS2C packet)
		{
			var cellX = mob.Movement.CellX;
			var cellY = mob.Movement.CellY;

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var c in _cells[cellPos.Item1, cellPos.Item2].LocalClients)
				{
					if (c == null)
						throw new NullReferenceException("null client");
					if (c.Character == null)
						throw new NullReferenceException("null client");

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

					if (c.Character == null)
						throw new NullReferenceException("null character");

					characters.Add(c.Character);
				}
			}
			return characters;
		}

		public List<Mob> GetNearbyMobs(Client client)
		{
			List<Mob> mobs = new List<Mob>();

			var cellX = client.Character.Location.Movement.CellX;
			var cellY = client.Character.Location.Movement.CellY;

			foreach (var cellPos in GetNeighbours((Int16)cellX, (Int16)cellY))
			{
				foreach (var m in _cells[cellPos.Item1, cellPos.Item2].LocalMobs)
				{
					if (m == null)
						throw new NullReferenceException("null mob");
					if (m.IsSpawned == false)
						throw new Exception("mob not spawned, shouldnt be in a cell");

					mobs.Add(m);
				}
			}
			return mobs;
		}

		internal void Update()
		{
			MobManager.UpdateAll();
		}
	}
}
