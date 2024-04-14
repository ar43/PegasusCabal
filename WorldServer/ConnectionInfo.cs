using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer
{
	internal class ConnectionInfo
	{
		public ConnectionInfo(UInt16 userId, UInt32 authKey)
		{
			UserId = userId;
			AuthKey = authKey;
		}

		public UInt16 UserId { get; private set; }
		public UInt32 AuthKey { get; private set; }

	}
}
