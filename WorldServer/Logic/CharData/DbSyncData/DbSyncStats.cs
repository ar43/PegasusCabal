using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncStats
	{
		public DbSyncStats(Int32 level, Int64 exp, Int32 axp, Int32 str, Int32 dex, Int32 @int, Int32 pnt, Int32 rank)
		{
			Level = level;
			Exp = exp;
			Axp = axp;
			Str = str;
			Dex = dex;
			Int = @int;
			Pnt = pnt;
			Rank = rank;
		}

		public int Level { get; private set; }
		public long Exp { get; private set; }
		public int Axp { get; private set; }
		public int Str { get; private set; }
		public int Dex { get; private set; }
		public int Int { get; private set; }
		public int Pnt { get; private set; }
		public int Rank { get; private set; }
	}
}
