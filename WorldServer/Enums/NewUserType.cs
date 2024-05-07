using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum NewUserType
	{
		OtherPlayers = 0,
		NewInit = 48,
		NewWarp,
		NewMove,
		NewResurrect
	}
}
