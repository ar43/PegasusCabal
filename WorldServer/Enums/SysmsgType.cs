using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum SysmsgType
	{
		SMT_NORMAL = 0x0,
		SMT_LOGINDUP,
		SMT_FDISCON,
		SMT_RDISCON,
		SMT_RDISCON2,
		SMT_NORMAL_IWAR_CAPELLA,
		SMT_NORMAL_IWAR_PROCYON,
	}
}
