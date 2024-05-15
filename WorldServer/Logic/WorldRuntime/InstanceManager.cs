using LibPegasus.Parsers.Mcl;
using Npgsql.TypeMapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.WorldRuntime.MapDataRuntime;
using WorldServer.Logic.WorldRuntime.WarpsRuntime;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.WorldRuntime
{
	internal class InstanceManager
	{
		private readonly Dictionary<UInt128, Instance> _instances;
		private readonly Dictionary<int, TileAttributeData> _tileAttributes;

		private readonly WorldConfig _worldConfig;
		private readonly WarpManager _warpManager;
		private readonly MapDataManager _mapDataManager;

		private Random _random;

		public InstanceManager(WorldConfig worldConfig, WarpManager warpManager, MapDataManager mapDataManager)
		{
			_warpManager = warpManager;
			_worldConfig = worldConfig;
			_mapDataManager = mapDataManager;

			_instances = new Dictionary<UInt128, Instance>();
			_tileAttributes = new Dictionary<int, TileAttributeData>();
			_random = new Random();

			AddInstance(Enums.MapId.BLOODY_ICE, InstanceType.PERMANENT);
			AddInstance(Enums.MapId.GREEN_DESPAIR, InstanceType.PERMANENT);
			AddInstance(Enums.MapId.DESERT_SCREAM, InstanceType.PERMANENT);
			AddInstance(Enums.MapId.WARP_CENTER, InstanceType.PERMANENT);
		}

		private void WarpClient(Client client, Instance newInstance, UInt32 warpType, int newX, int newY)
		{
			//this is a guess, but looks that way (emulates the randomized warp effect observed on first 4 maps on town tiles)
			if ((int)newInstance.MapId >= 1 && (int)newInstance.MapId <= 4 && newInstance.CheckTileTown((UInt16)newX, (UInt16)newY))
			{
				var cellX = (newX / 16);
				var cellY = (newY / 16);

				var list = newInstance.CalculateValidCellSpots((UInt16)cellX, (UInt16)cellY);

				var position = list[_random.Next(list.Count)];
				newX = position.Item1;
				newY = position.Item2;
			}

			var oldInstance = client.Character.Location.Instance;
			oldInstance.RemoveClient(client, DelUserType.WARP);

			client.Character.Location.Movement.SetPosition(newX, newY);
			client.Character.Location.Movement.UpdateCellPos();

			//todo: send ChargeInfo

			var response = new RSP_WarpCommand(client.Character, warpType, (UInt32)newInstance.MapId, 0);
			client.PacketManager.Send(response);

			AddClient(client, newInstance.Id, NewUserType.NEWWARP);
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

		public bool WarpClientByNpcId(Client client, int npcId)
		{
			var instance = client.Character.Location.Instance;

			if (!instance.MapData.NpcData.TryGetValue(npcId, out var npc))
				return false;
			if (!npc.NpcWarpData.TryGetValue(0, out var npcWarp))
				return false;
			var warpId = npcWarp.TargetId;
			var warp = _warpManager.Get(warpId);
			if(warp == null) return false;

			//TODO: Nation checking
			if(_instances.TryGetValue((UInt128)warp.WorldIdx, out var newInstance))
			{
				if (newInstance.Type != InstanceType.PERMANENT)
					return false;
				WarpClient(client, newInstance, 8, warp.PosXPnt, warp.PosYPnt);
				return true;
			}
			
			return false;
		}

		public void AddInstance(MapId mapId, InstanceType instanceType)
		{
			Instance instance = new(mapId, instanceType, _mapDataManager.Get((Int32)mapId));

			if(_instances.ContainsKey(instance.Id))
			{
				throw new Exception("Instance manager already contains an instance with that id");
			}

			instance.TileAttributeData = GetTileAttributeData((Int32)instance.MapId);


			_instances[instance.Id] = instance;
		}

		private TileAttributeData GetTileAttributeData(int mapId) //this needs to be async probably
		{
			if(_tileAttributes.ContainsKey(mapId))
			{
				return _tileAttributes[mapId];
			}
			else
			{
				var mapIdString = mapId.ToString("00");
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

		public void AddClient(Client client, UInt128 instanceId, NewUserType type)
		{
			_instances[instanceId].AddNewClient(client, (UInt16)client.Character.Location.Movement.CellX, (UInt16)client.Character.Location.Movement.CellY);
			if (client.Character.Location.Instance != null)
				throw new Exception("InstanceMngr AddClient not null");
			client.Character.Location.Instance = _instances[instanceId];

			var otherCharacters = client.Character.Location.Instance.GetNearbyCharacters(client);
			if (otherCharacters.Count > 0)
			{
				var packet_new_list_others = new NFY_NewUserList(otherCharacters, NewUserType.OTHERPLAYERS);
				client.PacketManager.Send(packet_new_list_others);
			}

			var packet_new_list_this = new NFY_NewUserList(new List<Character>() { client.Character }, type);
			client.BroadcastNearby(packet_new_list_this, true);
		}
	}
}
