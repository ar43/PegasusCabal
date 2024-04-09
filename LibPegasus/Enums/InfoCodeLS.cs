using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Enums
{
	public enum InfoCodeLS : UInt32
	{
		REGISTRATION_OK = 0,
		REGISTRATION_USEREXISTS,
		REGISTRATION_FAILED,
		LOGIN_SUCCESS,
		LOGIN_FAILED,
		SESSION_TIMEOUT
	}
}
