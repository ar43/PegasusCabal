using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums.Mob
{
	internal enum MobPhase
	{
		INVALID = 0,
		MOVE,
		FIND,
		BATTLE,
		CHASE
	}
}
