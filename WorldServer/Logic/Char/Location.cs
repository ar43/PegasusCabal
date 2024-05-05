using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Char
{
	internal class Location
	{
		public UInt16 X;
		public UInt16 Y;

		public UInt32 WorldId;

		public Location(UInt16 x, UInt16 y, UInt32 worldId)
		{
			X = x;
			Y = y;
			WorldId = worldId;
		}
	}
}
