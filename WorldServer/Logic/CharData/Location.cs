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
		

		public Instance? Instance;

		public MovementData Movement {get; private set;}

		public Location(UInt16 x, UInt16 y)
		{
			Movement = new MovementData(false, x, y, 4.5f);
		}

		
	}
}
