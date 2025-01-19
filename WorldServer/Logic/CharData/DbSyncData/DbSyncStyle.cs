using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncStyle
	{
		public DbSyncStyle(UInt32 styleSerial)
		{
			StyleSerial = styleSerial;
		}

		public uint StyleSerial { get; private set; }
	}
}
