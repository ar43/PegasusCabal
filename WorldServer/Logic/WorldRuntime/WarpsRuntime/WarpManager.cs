using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.WarpsRuntime
{
	internal class WarpManager
	{
		private Dictionary<int, Warp> _warps;

		public WarpManager(WorldConfig config)
		{
			_warps = new();
			var warpConfig = config.GetConfig("[Warp]");
			foreach (var item in warpConfig)
			{
				var warpId = Convert.ToInt32(item.Key);
				_ = int.TryParse(item.Value["WorldIdx"], out var worldId);
				_ = int.TryParse(item.Value["PosXPnt"], out var posXPnt);
				_ = int.TryParse(item.Value["PosYPnt"], out var posYPnt);
				_ = int.TryParse(item.Value["Nation1PosXPnt"], out var nation1PosXPnt);
				_ = int.TryParse(item.Value["Nation1PosYPnt"], out var nation1PosYPnt);
				_ = int.TryParse(item.Value["Nation2PosXPnt"], out var nation2PosXPnt);
				_ = int.TryParse(item.Value["Nation2PosYPnt"], out var nation2PosYPnt);
				_ = int.TryParse(item.Value["LV"], out var lvl);
				_ = int.TryParse(item.Value["Fee"], out var fee);
				Warp warp = new Warp(warpId, worldId, posXPnt, posYPnt, nation1PosXPnt, nation1PosYPnt, nation2PosXPnt, nation2PosYPnt, lvl, fee);

				if (!_warps.TryAdd(warpId, warp))
					throw new Exception("Warp with that id already exists");
			}
		}

		public Warp? Get(int warpId)
		{
			if(_warps.TryGetValue(warpId, out var warp))
			{
				return warp;
			}
			else
			{
				return null;
			}
		}
	}
}
