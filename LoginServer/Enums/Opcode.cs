using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Enums
{
	internal enum Opcode : UInt16
	{
		CONNECT2SVR = 101,
		AUTHACCOUNT = 103,
		SYSTEMMESSG = 120,
		SERVERSTATE = 121,
		CHECKVERSION = 122,
		URLTOCLIENT = 128,
		PUBLICKEY = 2001,
		PRESERVERENVREQUEST = 2002
	}
}
