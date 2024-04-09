using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Enums
{
	internal enum ConnState
	{
		INITIAL,
		CONNECTED,
		VERSION_CHECKED,
		PRE_ENV,
		PUBLIC_KEY_REQUESTED,
		AUTH_ACCOUNT
	}
}
