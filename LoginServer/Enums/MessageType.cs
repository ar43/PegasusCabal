using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Enums
{
	internal enum MessageType
	{
		Normal = 0,
		LoginDuplicate,
		ForceDisconnect,
		Shutdown,
		ShutdownNoNotice,
		War_Capella,
		War_Procyon,
		Normal2 = 9
	}
}
