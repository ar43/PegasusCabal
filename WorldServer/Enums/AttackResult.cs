using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Enums
{
	internal enum AttackResult
	{
		SR_CRITICAL = 0x01,
		SR_NORMALAK,
		SR_MISSINGS,
		SR_MOBSDEAD,

		// ���λ����
		SR_SUCCSBUF,
		SR_EXISTBUF = 0x10,
		SR_EXTCRITL = SR_EXISTBUF + SR_CRITICAL,
		SR_EXTNORML = SR_EXISTBUF + SR_NORMALAK,
		SR_EXTMISSN = SR_EXISTBUF + SR_MISSINGS,

		SR_ENCHANT,
		SR_STUNFAIL,
		SR_FULLFAIL,
		SR_DIFFWORLD,
		SR_TARGETERROR,
		SR_DMG_ABSORBED,
	}
}
