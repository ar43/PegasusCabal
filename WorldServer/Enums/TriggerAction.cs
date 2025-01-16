using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum TriggerAction
	{
		TA_SPAWN = 1,
		TA_KILL,
		TA_WAKE,
		TA_DELETE,
		TA_CHANGE,
		TA_EVENT_CALL,
		TA_END,
	}
}
