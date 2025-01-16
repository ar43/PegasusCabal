using LibPegasus.Utils;

namespace WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime
{
	internal class MissionDungeonDataManager
	{
		private readonly WorldConfig _config;
		public MissionDungeonDataManager(WorldConfig worldConfig)
		{
			MainData = new();
			_config = worldConfig;
			LoadConfig();
		}

		public Dictionary<int, MissionDungeonDataMain> MainData { get; private set; }

		private void LoadConfig()
		{
			MainData = new();
			var cfg = _config.GetConfig("[MD]");

			foreach (var it in cfg.Values)
			{
				int QDungeonIdx = Convert.ToInt32(it["QDungeonIdx"]);
				int InstanceLimit = Convert.ToInt32(it["InstanceLimit"]);
				int Level = Convert.ToInt32(it["Level"]);
				int MaxUser = Convert.ToInt32(it["MaxUser"]);
				int MissionTimeout = Convert.ToInt32(it["MissionTimeout"]);
				int[]? OpenItem = Utility.StringToIntArrayComplex(it["OpenItem"]);
				int PPLink = Convert.ToInt32(it["PPLink"]);
				int Penalty = Convert.ToInt32(it["Penalty"]);
				int[]? Reward = Utility.StringToIntArrayComplex(it["Reward"]);
				int WarpIdx = Convert.ToInt32(it["WarpIdx"]);
				int WarpIdxForSucess = Convert.ToInt32(it["WarpIdxForSucess"]);
				int WarpIdxForFail = Convert.ToInt32(it["WarpIdxForFail"]);
				int WarpIndexForDead = Convert.ToInt32(it["WarpIndexForDead"]);
				int WorldIdx = Convert.ToInt32(it["WorldIdx"]);
				int bAddRange = Convert.ToInt32(it["bAddRange"]);
				int NextQDIdxforSuccess = Convert.ToInt32(it["NextQDIdxForSuccess"]);
				int WarpNPC_Set = Convert.ToInt32(it["WarpNPC_Set"]);
				int UseTerrain = Convert.ToInt32(it["UseTerrain"]);
				int[]? BattleStyle = Utility.StringToIntArrayComplex(it["BattleStyle"]);
				int UseOddCircle_Count = Convert.ToInt32(it["UseOddCircle_Count"]);
				int Party_Type = Convert.ToInt32(it["Party_Type"]);
				int RemoveItem = Convert.ToInt32(it["RemoveItem"]);
				int DBWrite = Convert.ToInt32(it["DBWrite"]);
				int DungeonType = Convert.ToInt32(it["DungeonType"]);

				if (PPLink != QDungeonIdx)
					throw new Exception("PPLink does not equal QDungeonIdx...");

				var mdInfo = new MissionDungeonInfo(QDungeonIdx, InstanceLimit, Level, MaxUser, MissionTimeout, OpenItem,
					PPLink, Penalty, Reward, WarpIdx, WarpIdxForSucess, WarpIdxForFail,
					WarpIndexForDead, WorldIdx, bAddRange, NextQDIdxforSuccess, WarpNPC_Set, UseTerrain, BattleStyle, UseOddCircle_Count,
					Party_Type, RemoveItem, DBWrite, DungeonType);

				Add(QDungeonIdx, new MissionDungeonDataMain(mdInfo));

			}

			cfg = _config.GetConfig("[MMap]");
			foreach (var it in cfg.Values)
			{
				int MobIdx = Convert.ToInt32(it["MobIdx"]);
				int PPIdx = Convert.ToInt32(it["PPIdx"]);
				int SpeciesIdx = Convert.ToInt32(it["SpeciesIdx"]);
				int PosX = Convert.ToInt32(it["PosX"]);
				int PosY = Convert.ToInt32(it["PosY"]);
				int Width = Convert.ToInt32(it["Width"]);
				int Height = Convert.ToInt32(it["Height"]);
				int SpwnInterval = Convert.ToInt32(it["SpwnInterval"]);
				int SpwnCount = Convert.ToInt32(it["SpwnCount"]);
				int SpawnDefault = Convert.ToInt32(it["SpawnDefault"]);
				int[]? EvtProperty = Utility.StringToIntArrayComplex(it["EvtProperty"]);
				int[]? EvtMobs = Utility.StringToIntArrayComplex(it["EvtMobs"]);
				int[]? EvtInterval = Utility.StringToIntArrayComplex(it["EvtInterval"]);
				int Grade = Convert.ToInt32(it["Grade"]);
				int Lv = Convert.ToInt32(it["Lv"]);
				int MissionGate = Convert.ToInt32(it["MissionGate"]);
				int PerfectDrop = Convert.ToInt32(it["PerfectDrop"]);
				int TrgIdxSpawn = Convert.ToInt32(it["TrgIdxSpawn"]);
				int TrgIdxKill = Convert.ToInt32(it["TrgIdxKill"]);
				int Type = Convert.ToInt32(it["Type"]);
				int Min = Convert.ToInt32(it["Min"]);
				int Max = Convert.ToInt32(it["Max"]);
				int Authority = Convert.ToInt32(it["Authority"]);
				int Server_Mob = Convert.ToInt32(it["Server_Mob"]);
				int Loot_Delay = Convert.ToInt32(it["Loot_Delay"]);

				var mmap = new MissionDungeonMMap(MobIdx, PPIdx, SpeciesIdx, PosX, PosY, Width, Height, SpwnInterval,
					SpwnCount, SpawnDefault, EvtProperty, EvtMobs, EvtInterval, Grade, Lv, MissionGate,
					PerfectDrop, TrgIdxSpawn, TrgIdxKill, Type, Min, Max, Authority, Server_Mob, Loot_Delay);

				AddMMap(PPIdx, mmap);
			}

			cfg = _config.GetConfig("[Trg2Evt]");
			foreach (var it in cfg.Values)
			{
				int QDungeonIdx = Convert.ToInt32(it["QDungeonIdx"]);
				int TrgIdx = Convert.ToInt32(it["TrgIdx"]);
				int Order = Convert.ToInt32(it["Order"]);
				int TrgType = Convert.ToInt32(it["TrgType"]);
				string? LiveStateMMapIdx = it["LiveStateMMapIdx"];
				string? DeadStateMMapIdx = it["DeadStateMMapIdx"];
				int TrgNpcIdx = Convert.ToInt32(it["TrgNpcIdx"]);
				int EvtActGroupIdx = Convert.ToInt32(it["EvtActGroupIdx"]);

				var trigger = new MissionDungeonTrigger(QDungeonIdx, TrgIdx, Order, TrgType, LiveStateMMapIdx, DeadStateMMapIdx, TrgNpcIdx, EvtActGroupIdx);

				AddTrigger(QDungeonIdx, trigger);
			}

			cfg = _config.GetConfig("[PatternPart]");
			foreach (var it in cfg.Values)
			{
				int PPIdx = Convert.ToInt32(it["PPIdx"]);
				int[]? MissionMobs = Utility.StringToIntArrayComplex(it["MissionMobs"]);
				int MissionNPC = Convert.ToInt32(it["MissionNPC"]);
				AddPP(PPIdx, new MissionDungeonPP(PPIdx, MissionMobs, MissionNPC));
			}

			cfg = _config.GetConfig("[ActGroup]");
			foreach (var it in cfg.Values)
			{
				int QDungeonIdx = Convert.ToInt32(it["QDungeonIdx"]);
				int EvtActGroupIdx = Convert.ToInt32(it["EvtActGroupIdx"]);
				int Order = Convert.ToInt32(it["Order"]);
				int TgtMMapIdx = Convert.ToInt32(it["TgtMMapIdx"]);
				int TgtAction = Convert.ToInt32(it["TgtAction"]);
				int EvtDelay = Convert.ToInt32(it["EvtDelay"]);
				string TgtSpawnInterval = it["TgtSpawnInterval"];
				string TgtSpawnCount = it["TgtSpawnCount"];

				AddActGroup(QDungeonIdx, new(QDungeonIdx, EvtActGroupIdx, Order, TgtMMapIdx, TgtAction, EvtDelay, TgtSpawnInterval, TgtSpawnCount));
			}

		}

		private void AddMMap(int ppidx, MissionDungeonMMap mmap)
		{
			if (!MainData.ContainsKey(ppidx))
				throw new Exception();

			var mmapList = MainData[ppidx].MissionDungeonMMap;

			mmapList.Add(mmap);
		}

		private void AddPP(int ppidx, MissionDungeonPP pp)
		{
			if (!MainData.ContainsKey(ppidx))
				throw new Exception();

			if (MainData[ppidx].MissionDungeonPP != null)
				throw new Exception();

			MainData[ppidx].MissionDungeonPP = pp;
		}

		private void AddTrigger(int qDungeonIdx, MissionDungeonTrigger trigger)
		{
			if (!MainData.ContainsKey(qDungeonIdx))
				throw new Exception();

			var triggerList = MainData[qDungeonIdx].MissionDungeonTriggers;

			triggerList.Add(trigger);
		}

		private void AddActGroup(int qDungeonIdx, MissionDungeonActGroup actGroup)
		{
			if (!MainData.ContainsKey(qDungeonIdx))
				throw new Exception();

			var actList = MainData[qDungeonIdx].MissionDungeonActGroup;

			actList.Add(actGroup);
		}

		private void Add(int id, MissionDungeonDataMain mainInfo)
		{
			if (MainData.ContainsKey(id))
				throw new Exception();

			MainData.Add(id, mainInfo);
		}
	}

	internal class MissionDungeonDataMain
	{
		public MissionDungeonDataMain(MissionDungeonInfo missionDungeonInfo)
		{
			MissionDungeonInfo = missionDungeonInfo;
			MissionDungeonMMap = new();
			MissionDungeonTriggers = new();
			MissionDungeonActGroup = new();
		}

		public MissionDungeonInfo MissionDungeonInfo { get; private set; }
		public List<MissionDungeonMMap> MissionDungeonMMap { get; private set; }
		public List<MissionDungeonTrigger> MissionDungeonTriggers { get; private set; }
		public MissionDungeonPP? MissionDungeonPP { get; set; }
		public List<MissionDungeonActGroup> MissionDungeonActGroup { get; private set; }


	}

	internal class MissionDungeonInfo
	{
		public MissionDungeonInfo(Int32 qDungeonIdx, Int32 instanceLimit, Int32 level, Int32 maxUser, Int32 missionTimeout, Int32[]? openItem, Int32 pPLink, Int32 penalty, Int32[]? reward, Int32 warpIdx, Int32 warpIdxForSucess, Int32 warpIdxForFail, Int32 warpIndexForDead, Int32 worldIdx, Int32 bAddRange, Int32 nextQDIdxforSuccess, Int32 warpNPC_Set, Int32 useTerrain, Int32[]? battleStyle, Int32 useOddCircle_Count, Int32 party_Type, Int32 removeItem, Int32 dBWrite, Int32 dungeonType)
		{
			QDungeonIdx = qDungeonIdx;
			InstanceLimit = instanceLimit;
			Level = level;
			MaxUser = maxUser;
			MissionTimeout = missionTimeout;
			OpenItem = openItem;
			PPLink = pPLink;
			Penalty = penalty;
			Reward = reward;
			WarpIdx = warpIdx;
			WarpIdxForSucess = warpIdxForSucess;
			WarpIdxForFail = warpIdxForFail;
			WarpIndexForDead = warpIndexForDead;
			WorldIdx = worldIdx;
			this.bAddRange = bAddRange;
			NextQDIdxforSuccess = nextQDIdxforSuccess;
			WarpNPC_Set = warpNPC_Set;
			UseTerrain = useTerrain;
			BattleStyle = battleStyle;
			UseOddCircle_Count = useOddCircle_Count;
			Party_Type = party_Type;
			RemoveItem = removeItem;
			DBWrite = dBWrite;
			DungeonType = dungeonType;
		}

		public int QDungeonIdx { get; private set; }
		public int InstanceLimit { get; private set; }
		public int Level { get; private set; }
		public int MaxUser { get; private set; }
		public int MissionTimeout { get; private set; }
		public int[]? OpenItem { get; private set; }
		public int PPLink { get; private set; }
		public int Penalty { get; private set; }
		public int[]? Reward { get; private set; }
		public int WarpIdx { get; private set; }
		public int WarpIdxForSucess { get; private set; }
		public int WarpIdxForFail { get; private set; }
		public int WarpIndexForDead { get; private set; }
		public int WorldIdx { get; private set; }
		public int bAddRange { get; private set; }
		public int NextQDIdxforSuccess { get; private set; }
		public int WarpNPC_Set { get; private set; }
		public int UseTerrain { get; private set; }
		public int[]? BattleStyle { get; private set; }
		public int UseOddCircle_Count { get; private set; }
		public int Party_Type { get; private set; }
		public int RemoveItem { get; private set; }
		public int DBWrite { get; private set; }
		public int DungeonType { get; private set; }
	}

	internal class MissionDungeonMMap
	{
		public MissionDungeonMMap(Int32 mobIdx, Int32 pPIdx, Int32 speciesIdx, Int32 posX, Int32 posY, Int32 width, Int32 height, Int32 spwnInterval, Int32 spwnCount, Int32 spawnDefault, Int32[]? evtProperty, Int32[]? evtMobs, Int32[]? evtInterval, Int32 grade, Int32 lv, Int32 missionGate, Int32 perfectDrop, Int32 trgIdxSpawn, Int32 trgIdxKill, Int32 type, Int32 min, Int32 max, Int32 authority, Int32 server_Mob, Int32 loot_Delay)
		{
			MobIdx = mobIdx;
			PPIdx = pPIdx;
			SpeciesIdx = speciesIdx;
			PosX = posX;
			PosY = posY;
			Width = width;
			Height = height;
			SpwnInterval = spwnInterval;
			SpwnCount = spwnCount;
			SpawnDefault = spawnDefault;
			EvtProperty = evtProperty;
			EvtMobs = evtMobs;
			EvtInterval = evtInterval;
			Grade = grade;
			Lv = lv;
			MissionGate = missionGate;
			PerfectDrop = perfectDrop;
			TrgIdxSpawn = trgIdxSpawn;
			TrgIdxKill = trgIdxKill;
			Type = type;
			Min = min;
			Max = max;
			Authority = authority;
			Server_Mob = server_Mob;
			Loot_Delay = loot_Delay;
		}

		public int MobIdx { get; private set; }
		public int PPIdx { get; private set; }
		public int SpeciesIdx { get; private set; }
		public int PosX { get; private set; }
		public int PosY { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int SpwnInterval { get; private set; }
		public int SpwnCount { get; private set; }
		public int SpawnDefault { get; private set; }
		public int[]? EvtProperty { get; private set; }
		public int[]? EvtMobs { get; private set; }
		public int[]? EvtInterval { get; private set; }
		public int Grade { get; private set; }
		public int Lv { get; private set; }
		public int MissionGate { get; private set; }
		public int PerfectDrop { get; private set; }
		public int TrgIdxSpawn { get; private set; }
		public int TrgIdxKill { get; private set; }
		public int Type { get; private set; }
		public int Min { get; private set; }
		public int Max { get; private set; }
		public int Authority { get; private set; }
		public int Server_Mob { get; private set; }
		public int Loot_Delay { get; private set; }

	}

	internal class MissionDungeonTrigger
	{
		public MissionDungeonTrigger(Int32 qDungeonIdx, Int32 trgIdx, Int32 order, Int32 trgType, String? liveStateMMapIdx, String? deadStateMMapIdx, Int32 trgNpcIdx, Int32 evtActGroupIdx)
		{
			QDungeonIdx = qDungeonIdx;
			TrgIdx = trgIdx;
			Order = order;
			TrgType = trgType;
			LiveStateMMapIdx = liveStateMMapIdx;
			DeadStateMMapIdx = deadStateMMapIdx;
			TrgNpcIdx = trgNpcIdx;
			EvtActGroupIdx = evtActGroupIdx;
		}

		public int QDungeonIdx { get; private set; }
		public int TrgIdx { get; private set; }
		public int Order { get; private set; }
		public int TrgType { get; private set; }
		public string? LiveStateMMapIdx { get; private set; }
		public string? DeadStateMMapIdx { get; private set; }
		public int TrgNpcIdx { get; private set; }
		public int EvtActGroupIdx { get; private set; }
	}

	internal class MissionDungeonPP
	{
		public MissionDungeonPP(Int32 pPIdx, Int32[]? missionMobs, Int32 missionNPC)
		{
			PPIdx = pPIdx;
			MissionMobs = missionMobs;
			MissionNPC = missionNPC;
		}

		public int PPIdx { get; private set; }
		public int[]? MissionMobs { get; private set; }
		public int MissionNPC { get; private set; }
	}

	internal class MissionDungeonActGroup
	{
		public MissionDungeonActGroup(Int32 qDungeonIdx, Int32 evtActGroupIdx, Int32 order, Int32 tgtMMapIdx, Int32 tgtAction, Int32 evtDelay, string tgtSpawnInterval, string tgtSpawnCount)
		{
			QDungeonIdx = qDungeonIdx;
			EvtActGroupIdx = evtActGroupIdx;
			Order = order;
			TgtMMapIdx = tgtMMapIdx;
			TgtAction = tgtAction;
			EvtDelay = evtDelay;
			TgtSpawnInterval = tgtSpawnInterval;
			TgtSpawnCount = tgtSpawnCount;
		}

		public int QDungeonIdx { get; private set; }
		public int EvtActGroupIdx { get; private set; }
		public int Order { get; private set; }
		public int TgtMMapIdx { get; private set; }
		public int TgtAction { get; private set; }
		public int EvtDelay { get; private set; }
		public string TgtSpawnInterval { get; private set; }
		public string TgtSpawnCount { get; private set; }
	}
}
