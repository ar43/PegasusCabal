using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum ItemLootingResult
	{
		SUCCESS = 96,
		NOT_OWNER,
		TOO_LATE,
		SLOT_ALREADY_IN_USE,
		ANTI_ONLINE,
		OUT_OF_RANGE
	}
}
