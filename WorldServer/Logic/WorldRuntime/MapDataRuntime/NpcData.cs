using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class NpcData
	{
		public NpcData(Int32 index, Int32 flags, Int32 posX, Int32 posY, Int32 type, Boolean isRangeCheck)
		{
			Index = index;
			Flags = flags;
			PosX = posX;
			PosY = posY;
			Type = type;
			IsRangeCheck = isRangeCheck;
			NpcWarpData = new();
		}

		public int Index { get; private set; }
		public int Flags { get; private set; }
		public int PosX { get; private set; }
		public int PosY { get; private set;}
		public int Type { get; private set; }
		public bool IsRangeCheck { get; private set; }
		public Dictionary<int, NpcWarpData> NpcWarpData { get; private set; }
	}
}
