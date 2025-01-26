using LibPegasus.Utils;
using System.Globalization;
using System.Text.RegularExpressions;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MapDataManager
	{
		Dictionary<int, MapData> _maps;
		WorldConfig _config;
		MobDataManager _mobDataManager;

		public static readonly Dictionary<MapId, int> MapIdToMcl = new Dictionary<MapId, int>
		{
			{ MapId.BLOODY_ICE_EX, 1 },
			{ MapId.BLOODY_ICE, 1 },
			{ MapId.DESERT_SCREAM, 2 },
			{ MapId.GREEN_DESPAIR, 3 },
			{ MapId.WARP_CENTER, 30 },
			{ MapId.PORT_LUX, 4 },
			{ MapId.DUNGEON_WORLD_1, 29 },
			{ MapId.DUNGEON_WORLD_2, 28 },
			{ MapId.DUNGEON_WORLD_3, 27 },
			{ MapId.WARP_CENTER_EX, 30 },
		};

		public MapDataManager(WorldConfig config, MobDataManager mobDataManager)
		{
			_maps = new();
			_config = config;
			_mobDataManager = mobDataManager;
		}

		public MapData Get(int mapId)
		{
			if (_maps.TryGetValue(mapId, out var map))
			{
				return map;
			}
			else
			{
				var configStr = "[Terrain" + Convert.ToString(mapId) + "]";
				var subConfig = _config.GetConfig(configStr);
				if (subConfig != null)
				{
					var terrainData = subConfig["0"];
					if (subConfig.Count > 1)
						throw new NotImplementedException("Multi terrain map - figure it out: " + mapId.ToString());
					if (terrainData != null)
					{
						var terrainX = Convert.ToInt32(terrainData["TerrainX"]);
						var terrainY = Convert.ToInt32(terrainData["TerrainY"]);
						var warpIdxForDead = Convert.ToInt32(terrainData["WarpIdxForDead"]);
						var warpIdxForRetn = Convert.ToInt32(terrainData["WarpIdxForRetn"]);
						var warpIdxForLOut = Convert.ToInt32(terrainData["WarpIdxForLOut"]);
						var dmgMin = Convert.ToInt32(terrainData["DmgMin"]);
						var dmgMax = Convert.ToInt32(terrainData["DmgMax"]);
						var warControl = Convert.ToInt32(terrainData["WarControl"]);


						var terrainInfo = new TerrainInfo(terrainX, terrainY, warpIdxForDead, warpIdxForRetn, warpIdxForLOut, dmgMin, dmgMax, warControl);

						Dictionary<int, MobSpawnData> mobSpawnData = new();
						configStr = "[MMap" + Convert.ToString(mapId) + "]";
						subConfig = _config.GetConfig(configStr);
						foreach (var entry in subConfig)
						{
							var spawnId = Convert.ToInt32(entry.Key);
							int SpeciesIdx = Convert.ToInt32(entry.Value["SpeciesIdx"]);
							MobData MobData = _mobDataManager.Get(SpeciesIdx);
							int PosX = Convert.ToInt32(entry.Value["PosX"]);
							int PosY = Convert.ToInt32(entry.Value["PosY"]);
							int Width = Convert.ToInt32(entry.Value["Width"]);
							int Height = Convert.ToInt32(entry.Value["Height"]);
							int SpwnInterval = Convert.ToInt32(entry.Value["SpwnInterval"]);
							int SpawnDefault = Convert.ToInt32(entry.Value["SpawnDefault"]);
							int[]? EvtProperty = Utility.StringToIntArrayComplex(entry.Value["EvtProperty"]);
							int[]? EvtMobs = Utility.StringToIntArrayComplex(entry.Value["EvtMobs"]);
							int[]? EvtInterval = Utility.StringToIntArrayComplex(entry.Value["EvtInterval"]);
							int MissionGate = Convert.ToInt32(entry.Value["MissionGate"]);
							int PerfectDrop = Convert.ToInt32(entry.Value["PerfectDrop"]);
							int Type = Convert.ToInt32(entry.Value["Type"]);
							int Min = Convert.ToInt32(entry.Value["Min"]);
							int Max = Convert.ToInt32(entry.Value["Max"]);
							int Authority = Convert.ToInt32(entry.Value["Authority"]);
							int Server_Mob = Convert.ToInt32(entry.Value["Server_Mob"]);
							int Loot_Delay = Convert.ToInt32(entry.Value["Loot_Delay"]);
							mobSpawnData.Add(spawnId, new MobSpawnData(SpeciesIdx, MobData, PosX, PosY, Width, Height, SpwnInterval, SpawnDefault,
								EvtProperty, EvtMobs, EvtInterval,
								MissionGate, PerfectDrop, Type, Min, Max, Authority, Server_Mob, Loot_Delay));
						}

						Dictionary<(int, int, int), MissionDropData> localMissionDropData = new();
						configStr = "[MobsDropMission" + Convert.ToString(mapId) + "]";
						subConfig = _config.GetConfig(configStr);
						foreach (var missionDrop in subConfig.Values)
						{
							int TerrainIdx = Convert.ToInt32(missionDrop["TerrainIdx"]);
							int SpeciesIdx = Convert.ToInt32(missionDrop["SpeciesIdx"]);
							int ItemKind = Convert.ToInt32(missionDrop["ItemKind"]);
							int ItemOpt = Convert.ToInt32(missionDrop["ItemOpt"]);
							int DropRate = Convert.ToInt32(missionDrop["DropRate"]);
							int MaxDropCnt = Convert.ToInt32(missionDrop["MaxDropCnt"]);
							localMissionDropData.Add((SpeciesIdx, ItemKind, ItemOpt), new(TerrainIdx, SpeciesIdx, ItemKind, ItemOpt, DropRate, MaxDropCnt));
						}

						Dictionary<int, List<LocalDropData>> localDropTable = new();
						configStr = "[CommDrop" + Convert.ToString(mapId) + "]";
						subConfig = _config.GetConfig(configStr);
						foreach (var localDrop in subConfig.Values)
						{
							int TerrainIdx = Convert.ToInt32(localDrop["TerrainIdx"]);
							int ItemKind = Convert.ToInt32(localDrop["ItemKind"]);
							int ItemOpt = Convert.ToInt32(localDrop["ItemOpt"]);
							int DropRate = (int)((Convert.ToDouble(localDrop["DropRate"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0) * Int32.MaxValue);
							if (DropRate <= 0)
								throw new Exception("unexpected DropRate");
							int MinLv = Convert.ToInt32(localDrop["MinLv"]);
							int MaxLv = Convert.ToInt32(localDrop["MaxLv"]);
							int Group = Convert.ToInt32(localDrop["Group"]);
							int MaxDropCnt = Convert.ToInt32(localDrop["MaxDropCnt"]);
							int OptPoolIdx = Convert.ToInt32(localDrop["OptPoolIdx"]);
							int DurationIdx = Convert.ToInt32(localDrop["DurationIdx"]);

							if (Group != 0 && Group != ((MinLv + 5) / 13) + 1)
							{
								throw new Exception("unexpected group");
							}

							if (!localDropTable.ContainsKey(Group))
								localDropTable[Group] = new();
							localDropTable[Group].Add(new(TerrainIdx, ItemKind, ItemOpt, DropRate, MinLv, MaxLv, Group, MaxDropCnt, OptPoolIdx, DurationIdx));
						}

						Dictionary<int, List<MobDropData>> mobDropTable = new();
						configStr = "[MobsDrop" + Convert.ToString(mapId) + "]";
						subConfig = _config.GetConfig(configStr);
						foreach (var mobDrop in subConfig.Values)
						{
							int TerrainIdx = Convert.ToInt32(mobDrop["TerrainIdx"]);
							int SpeciesIdx = Convert.ToInt32(mobDrop["SpeciesIdx"]);
							int ItemKind = Convert.ToInt32(mobDrop["ItemKind"]);
							int ItemOpt = Convert.ToInt32(mobDrop["ItemOpt"]);
							int DropRate = (int)((Convert.ToDouble(mobDrop["DropRate"].Insert(0, "0"), new CultureInfo("en-US")) / 100.0) * Int32.MaxValue);
							if (DropRate <= 0)
								throw new Exception("unexpected DropRate");
							int MinLv = Convert.ToInt32(mobDrop["MinLv"]);
							int MaxDropCnt = Convert.ToInt32(mobDrop["MaxDropCnt"]);
							int OptPoolIdx = Convert.ToInt32(mobDrop["OptPoolIdx"]);
							int DurationIdx = Convert.ToInt32(mobDrop["DurationIdx"]);

							if (!mobDropTable.ContainsKey(SpeciesIdx))
								mobDropTable[SpeciesIdx] = new();
							mobDropTable[SpeciesIdx].Add(new(TerrainIdx, SpeciesIdx, ItemKind, ItemOpt, DropRate, MinLv, MaxDropCnt, OptPoolIdx, DurationIdx));
						}

						Dictionary<int, NpcData> npcData = new();
						configStr = "[NpcPos" + Convert.ToString(mapId) + "]";
						subConfig = _config.GetConfig(configStr);
						foreach (var npcCfg in subConfig.Values)
						{
							var flags = Convert.ToInt32(npcCfg["Flags"]);
							var index = Convert.ToInt32(npcCfg["Index"]);
							var posX = Convert.ToInt32(npcCfg["PosX"]);
							var posY = Convert.ToInt32(npcCfg["PosY"]);
							var type = Convert.ToInt32(npcCfg["Type"]);
							var isRangeCheck = Convert.ToBoolean(Convert.ToInt32((npcCfg["IsRangeCheck"])));
							var npcObj = new NpcData(index, flags, posX, posY, type, isRangeCheck);
							if (!npcData.TryAdd(index, npcObj))
								throw new Exception("Npc already exists");
						}

						configStr = "[WarpLst" + Convert.ToString(mapId) + "]";
						subConfig = _config.GetConfig(configStr);
						foreach (var warpCfg in subConfig.Values)
						{
							var npcId = Convert.ToInt32(warpCfg["NpcsIdx"]);
							var setId = Convert.ToInt32(warpCfg["NSetIdx"]);
							var targetId = Convert.ToInt32(warpCfg["TargetIdx"]);
							var level = Convert.ToInt32(warpCfg["LV"]);
							var fee = Convert.ToInt32(warpCfg["Fee"]);
							var type = Convert.ToInt32(warpCfg["Type"]);
							if (!npcData[npcId].NpcWarpData.TryAdd(setId, new(setId, level, fee, type, targetId)))
								throw new Exception("Npc warp already exists");
						}

						var npcPoolData = _config.GetConfig("[NPC]");
						foreach (var npcPool in npcPoolData.Values)
						{
							var worldId = Convert.ToInt32(npcPool["World_ID"]);
							if (worldId != mapId)
								continue;
							var npcId = Convert.ToInt32(npcPool["NPC_ID"]);
							var pool1Idx = Convert.ToInt32(npcPool["Pool_ID1"]);
							var pool2Idx = Convert.ToInt32(npcPool["Pool_ID2"]);
							if (npcData[npcId].Shop == null)
							{
								npcData[npcId].Shop = new(pool1Idx, pool2Idx);
							}
							else
							{
								throw new Exception("Shop already defined for that npc");
							}
						}

						var mapData = new MapData(mapId, terrainInfo, npcData, mobSpawnData, localMissionDropData, localDropTable, mobDropTable);
						if (!_maps.TryAdd(mapId, mapData))
							throw new Exception($"Map {mapId} already exists");
						return mapData;
					}
					else
					{
						throw new Exception(configStr + " does not exist");
					}
				}
				else
				{
					throw new Exception(configStr + " does not exist");
				}
			}
		}
	}
}
