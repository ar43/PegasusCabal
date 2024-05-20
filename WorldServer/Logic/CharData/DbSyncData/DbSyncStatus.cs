using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncStatus
	{
		public DbSyncStatus(Int32 hp, Int32 maxHp, Int32 mp, Int32 maxMp, Int32 sp, Int32 maxSp)
		{
			Hp = hp;
			MaxHp = maxHp;
			Mp = mp;
			MaxMp = maxMp;
			Sp = sp;
			MaxSp = maxSp;
		}

		public int Hp { get; private set; }
		public int MaxHp { get; private set; }
		public int Mp { get; private set; }
		public int MaxMp { get; private set; }
		public int Sp { get; private set; }
		public int MaxSp { get; private set; }
	}
}
