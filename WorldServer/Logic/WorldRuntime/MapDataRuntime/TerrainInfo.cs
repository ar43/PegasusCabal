using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class TerrainInfo
	{
		public TerrainInfo(Int32 terrainX, Int32 terrainY, Int32 warpIdxForDead, Int32 warpIdxForRetn, Int32 warpIdxForLOut, Int32 dmgMin, Int32 dmgMax, Int32 warControl)
		{
			TerrainX = terrainX;
			TerrainY = terrainY;
			WarpIdxForDead = warpIdxForDead;
			WarpIdxForRetn = warpIdxForRetn;
			WarpIdxForLOut = warpIdxForLOut;
			DmgMin = dmgMin;
			DmgMax = dmgMax;
			WarControl = warControl;
		}

		public int TerrainX { get; private set; }
		public int TerrainY { get; private set; }
		public int WarpIdxForDead { get; private set; }
		public int WarpIdxForRetn { get; private set; }
		public int WarpIdxForLOut { get; private set; }
		public int DmgMin { get; private set; }
		public int DmgMax { get; private set; }
		public int WarControl { get; private set; }
	}
}
