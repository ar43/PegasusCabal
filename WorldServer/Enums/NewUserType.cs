using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum NewUserType
	{
		OTHERPLAYERS = 0,
		NEWINIT = 48,
		NEWWARP,
		NEWMOVE,
		NEWRESURRECT
	}
}
