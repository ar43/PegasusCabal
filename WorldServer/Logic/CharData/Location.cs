using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.World;

namespace WorldServer.Logic.CharData
{
	internal class Location
	{
		public UInt16 X;
		public UInt16 Y;

		public UInt16 TileX;
		public UInt16 TileY;

		public Instance? Instance;

		public Location(UInt16 x, UInt16 y)
		{
			X = x;
			Y = y;
			TileX = (UInt16)(X / 16);
			TileY = (UInt16)(Y / 16);
		}
	}
}
