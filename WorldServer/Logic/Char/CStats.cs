using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Char
{
	internal class CStats
	{
		public CStats(UInt32 level, UInt32 exp, UInt32 str, UInt32 dex, UInt32 @int, UInt32 pnt, UInt32 rank)
		{
			Level = level;
			Exp = exp;
			Str = str;
			Dex = dex;
			Int = @int;
			Pnt = pnt;
			Rank = rank;
		}

		public UInt32 Level { get; private set; }
		public UInt32 Exp { get; private set; }
		public UInt32 Str { get; private set; }
		public UInt32 Dex { get; private set; }
		public UInt32 Int { get; private set; }
		public UInt32 Pnt { get; private set; }
		public UInt32 Rank { get; private set; }
	}
}
