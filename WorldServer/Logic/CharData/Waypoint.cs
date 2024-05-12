using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal struct Waypoint
	{
		public int X;
		public int Y;

		public Waypoint(Int32 x, Int32 y)
		{
			X = x;
			Y = y;
		}
	}
}
