using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Enums
{
	internal enum Opcode : UInt16
	{
		CONNECT2SVR = 101,
		AUTHACCOUNT = 103,
		CHECKVERSION = 122,
		PUBLICKEY = 2001,
		PRESERVERENVREQUEST = 2002
	}
}
