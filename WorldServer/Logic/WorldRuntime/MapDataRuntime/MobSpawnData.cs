using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.WorldRuntime.MobDataRuntime;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class MobSpawnData
	{
		public MobSpawnData(Int32 speciesIdx, MobData mobData, Int32 posX, Int32 posY, Int32 width, Int32 height, Int32 spwnInterval, Int32 spawnDefault, String evtProperty, String evtMobs, String evtInterval, Int32 missionGate, Int32 perfectDrop, Int32 type, Int32 min, Int32 max, Int32 authority, Int32 server_Mob, Int32 loot_Delay)
		{
			SpeciesIdx = speciesIdx;
			MobData = mobData;
			PosX = posX;
			PosY = posY;
			Width = width;
			Height = height;
			SpwnInterval = spwnInterval;
			SpawnDefault = spawnDefault;
			EvtProperty = evtProperty;
			EvtMobs = evtMobs;
			EvtInterval = evtInterval;
			MissionGate = missionGate;
			PerfectDrop = perfectDrop;
			Type = type;
			Min = min;
			Max = max;
			Authority = authority;
			Server_Mob = server_Mob;
			Loot_Delay = loot_Delay;
		}

		public int SpeciesIdx { get; private set; }
		public MobData MobData { get; private set; }
		public int PosX { get; private set; }
		public int PosY { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int SpwnInterval { get; private set; }
		public int SpawnDefault { get; private set; }
		public string EvtProperty { get; private set; }
		public string EvtMobs { get; private set; }
		public string EvtInterval { get; private set; }
		public int MissionGate { get; private set; }
		public int PerfectDrop { get; private set; }
		public int Type { get; private set; }
		public int Min { get; private set; }
		public int Max { get; private set; }
		public int Authority { get; private set; }
		public int Server_Mob { get; private set; }
		public int Loot_Delay { get; private set; }
	}
}
