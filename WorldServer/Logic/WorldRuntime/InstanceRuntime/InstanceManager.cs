using LibPegasus.Parsers.Mcl;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.Delegates;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.GroundItemRuntime;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MissionDungeonRuntime;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime;
using WorldServer.Logic.WorldRuntime.WarpsRuntime;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime
{
	internal class InstanceManager
	{
		private readonly Dictionary<UInt64, Instance> _instances;
		private readonly Dictionary<int, TileAttributeData> _tileAttributes;

		private readonly WorldConfig _worldConfig;
		private readonly WarpManager _warpManager;
		private readonly MapDataManager _mapDataManager;
		private readonly MissionDungeonDataManager _missionDungeonDataManager;

		private Random _random;

		public InstanceManager(WorldConfig worldConfig, WarpManager warpManager, MapDataManager mapDataManager, MissionDungeonDataManager missionDungeonDataManager)
		{
			_warpManager = warpManager;
			_worldConfig = worldConfig;
			_mapDataManager = mapDataManager;
			_missionDungeonDataManager = missionDungeonDataManager;

			_instances = new Dictionary<UInt64, Instance>();
			_tileAttributes = new Dictionary<int, TileAttributeData>();
			_random = new Random();

			AddFieldInstance(MapId.BLOODY_ICE, InstanceDuration.PERMANENT);
			AddFieldInstance(MapId.GREEN_DESPAIR, InstanceDuration.PERMANENT);
			AddFieldInstance(MapId.DESERT_SCREAM, InstanceDuration.PERMANENT);
			AddFieldInstance(MapId.WARP_CENTER, InstanceDuration.PERMANENT);
		}

		private void WarpClient(Client client, Instance newInstance, UInt32 warpType, int newX, int newY)
		{
			//this is a guess, but looks that way (emulates the randomized warp effect observed on first 4 maps on town tiles)
			if ((int)newInstance.MapId >= 1 && (int)newInstance.MapId <= 4 && newInstance.CheckTileTown((UInt16)newX, (UInt16)newY))
			{
				var cellX = newX / 16;
				var cellY = newY / 16;

				var list = newInstance.CalculateValidCellSpots((UInt16)cellX, (UInt16)cellY, true);

				var position = list[_random.Next(list.Count)];
				newX = position.Item1;
				newY = position.Item2;
			}

			var oldInstance = client.Character.Location.Instance;
			oldInstance.RemoveClient(client, DelObjectType.WARP);

			client.Character.Location.Movement.SetPosition(newX, newY);
			client.Character.Location.Movement.UpdateCellPos();

			var dgId = newInstance.Type == InstanceType.FIELD ? 0 : newInstance.MissionDungeonManager.GetDungeonId();

			//todo: send ChargeInfo

			var response = new RSP_WarpCommand(client.Character, warpType, (UInt32)newInstance.MapId, (uint)dgId);
			client.PacketManager.Send(response);

			AddClient(client, newInstance.Id, AddObjectType.NEWWARP);
		}

		public bool WarpClientReturn(Client client)
		{
			var instance = client.Character.Location.Instance;

			var warpId = instance.MapData.TerrainInfo.WarpIdxForRetn;
			var warp = _warpManager.Get(warpId);
			if (warp == null)
			{
				return false;
			}

			//TODO: Nation checking
			WarpClient(client, instance, 8, warp.PosXPnt, warp.PosYPnt);
			return true;
		}

		public bool WarpClientAbsolute(Client client, int x, int y)
		{
			var instance = client.Character.Location.Instance;

			if (instance == null)
				return false;

			//TODO: Nation checking
			WarpClient(client, instance, 8, x, y);
			return true;
		}

		public bool WarpClientByNpcId(Client client, int npcId)
		{
			var instance = client.Character.Location.Instance;

			if (!instance.MapData.NpcData.TryGetValue(npcId, out var npc))
				return false;
			if (!npc.NpcWarpData.TryGetValue(0, out var npcWarp))
				return false;
			

			if(npcWarp.Type == 0)
			{
				var warpId = npcWarp.TargetId;
				var warp = _warpManager.Get(warpId);
				if (warp == null) return false;
				//TODO: Nation checking
				if (_instances.TryGetValue((UInt64)warp.WorldIdx, out var newInstance))
				{
					if (newInstance.DurationType != InstanceDuration.PERMANENT)
						return false;
					WarpClient(client, newInstance, 8, warp.PosXPnt, warp.PosYPnt);
					return true;
				}
			}
			else
			{
				Debug.Assert(npcWarp.Type == 1 || npcWarp.Type == 2);
				var dgId = client.Character.Location.PendingDungeon.DungeonId;
				var instanceId = client.Character.Location.PendingDungeon.DungeonInstanceId;
				var warpDungeon = npcWarp.TargetId;

				if (instanceId == 0 || dgId != warpDungeon)
					return false;

				
				if (_instances.TryGetValue((ulong)instanceId, out var newInstance))
				{
					if (newInstance.DurationType != InstanceDuration.TEMP || newInstance.Type != InstanceType.DUNGEON)
						return false;

					var warpId = newInstance.MissionDungeonManager.GetStartWarpId();
					var warp = _warpManager.Get(warpId);

					WarpClient(client, newInstance, 2, warp.PosXPnt, warp.PosYPnt);
					client.Character.Location.PendingDungeon.Clear();
					return true;
				}
			}
			

			return false;
		}

		public Instance AddFieldInstance(MapId mapId, InstanceDuration instanceDuration)
		{
			Instance instance = new(mapId, instanceDuration, _mapDataManager.Get((Int32)mapId), InstanceType.FIELD);

			if (_instances.ContainsKey(instance.Id))
			{
				throw new Exception("Instance manager already contains an instance with that id");
			}

			instance.TileAttributeData = GetTileAttributeData((Int32)instance.MapId);
			instance.MobManager = new MobManager(instance, true);
			instance.GroundItemManager = new GroundItemManager(instance);


			_instances[instance.Id] = instance;
			return instance;
		}

		public Instance? AddDungeonInstance(MapId mapId, int dungeonId)
		{
			Instance instance = new(mapId, InstanceDuration.TEMP, _mapDataManager.Get((Int32)mapId), InstanceType.DUNGEON);

			if (_instances.ContainsKey(instance.Id))
			{
				throw new Exception("Instance manager already contains an instance with that id");
			}

			if(_missionDungeonDataManager.MainData.TryGetValue(dungeonId, out var dungeonData)) 
			{
				instance.TileAttributeData = GetTileAttributeData((Int32)instance.MapId);
				instance.MobManager = new MobManager(instance, false);
				instance.GroundItemManager = new GroundItemManager(instance);
				instance.MissionDungeonManager = new MissionDungeonManager(dungeonData);

				_instances[instance.Id] = instance;
				return instance;
			}
			else
			{
				return null;
			}

			
		}

		private TileAttributeData GetTileAttributeData(int mapId) //this needs to be async probably
		{
			if (_tileAttributes.ContainsKey(mapId))
			{
				return _tileAttributes[mapId];
			}
			else
			{
				var mapIdString = MapDataManager.MapIdToMcl[(MapId)mapId].ToString("00");
				if(mapIdString == null)
					throw new Exception($"{mapId} not defined in dictionary.");

				string workingDirectory = Environment.CurrentDirectory;
				string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
				var path = $"{projectDirectory}\\LibPegasus\\Data\\Maps\\mcl\\world_{mapIdString}.mcl";
				Serilog.Log.Information($"Loading map {path}");
				MclParser mclParser = new MclParser();
				mclParser.Parse(path);
				_tileAttributes.Add(mapId, mclParser.AttributeData);
				return _tileAttributes[mapId];
			}
		}

		public void AddClient(Client client, UInt64 instanceId, AddObjectType type)
		{
			_instances[instanceId].AddNewClient(client, (UInt16)client.Character.Location.Movement.CellX, (UInt16)client.Character.Location.Movement.CellY);
			if (client.Character.Location.Instance != null)
				throw new Exception("InstanceMngr AddClient not null");
			client.Character.Location.Instance = _instances[instanceId];

			var otherCharacters = client.Character.Location.Instance.GetNearbyCharacters(client);
			if (otherCharacters.Count > 0)
			{
				var packetNewListOtherPlayers = new NFY_NewUserList(otherCharacters, AddObjectType.OTHERPLAYERS);
				client.PacketManager.Send(packetNewListOtherPlayers);
			}

			var mobList = client.Character.Location.Instance.GetNearbyMobs(client);
			if (mobList.Count > 0)
			{
				var packetMobs = new NFY_NewMobsList(mobList);
				client.PacketManager.Send(packetMobs);
			}

			var groundItemList = client.Character.Location.Instance.GetNearbyGroundItems(client);
			if (groundItemList.Count > 0)
			{
				var packetItems = new NFY_NewItemList(groundItemList, 0, 0xFFFFFFFF);
				client.PacketManager.Send(packetItems);
			}

			var packet_new_list_this = new NFY_NewUserList(new List<Character>() { client.Character }, type);
			client.BroadcastNearby(packet_new_list_this, true);
		}

		internal void Update()
		{
			foreach (var instance in _instances.Values)
			{
				if (instance == null)
					continue;
				instance.Update();
			}
		}
	}
}
