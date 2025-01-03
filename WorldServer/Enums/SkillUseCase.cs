using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum SkillUseCase
	{
		ENABLE_BM1_2HND = 0x0001,
		ENABLE_BM2_2HND = 0x0002,

		ENABLE_BM1_DUAL = 0x0004,
		ENABLE_BM2_DUAL = 0x0008,

		ENABLE_BM1_MAIC = 0x0010,
		ENABLE_BM2_MAIC = 0x0020,

		ENABLE_BM1_MARR = 0x0040,
		ENABLE_BM2_MARR = 0x0080,

		ENABLE_BM1_SSHD = 0x0100,
		ENABLE_BM2_SSHD = 0x0200,

		ENABLE_BM1_MSWD = 0x0400,
		ENABLE_BM2_MSWD = 0x0800,

		ENABLE_NORMAL = 0x1000,
		ENABLE_ASTRAL = 0x2000,

		ENABLE_ALL_USE = 0x3FFF,
	}
}
