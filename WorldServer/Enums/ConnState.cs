using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum ConnState : UInt16
	{
		UNCONNECTED,
		AWAITING,
		AUTHORIZING,
		CONNECTED,
		KICKED,
		TIMEOUT,
		ERROR,
		AWAITING_LINK_REPLY,
		EXITED
	}
}
