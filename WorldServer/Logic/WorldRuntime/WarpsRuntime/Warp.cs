using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.WarpsRuntime
{
	internal class Warp
	{
		public Warp(int warpId, Int32 worldIdx, Int32 posXPnt, Int32 posYPnt, Int32 nation1PosXPnt, Int32 nation1PosYPnt, Int32 nation2PosXPnt, Int32 nation2PosYPnt, Int32 lvl, Int32 fee)
		{
			WarpId = warpId;
			WorldIdx = worldIdx;
			PosXPnt = posXPnt;
			PosYPnt = posYPnt;
			Nation1PosXPnt = nation1PosXPnt;
			Nation1PosYPnt = nation1PosYPnt;
			Nation2PosXPnt = nation2PosXPnt;
			Nation2PosYPnt = nation2PosYPnt;
			Lvl = lvl;
			Fee = fee;
		}

		public int WarpId { get; private set; }
		public int WorldIdx {  get; private set; }
		public int PosXPnt { get; private set; }
		public int PosYPnt { get; private set; }
		public int Nation1PosXPnt { get; private set; }
		public int Nation1PosYPnt { get; private set; }
		public int Nation2PosXPnt { get; private set; }
		public int Nation2PosYPnt { get; private set; }
		public int Lvl { get; private set; }
		public int Fee { get; private set; }
	}
}
