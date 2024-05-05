using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Char
{
	internal class CStatus
	{
		public CStatus(UInt32 hp, UInt32 maxHp, UInt32 mp, UInt32 maxMp, UInt32 sp, UInt32 maxSP)
		{
			Hp = hp;
			MaxHp = maxHp;
			Mp = mp;
			MaxMp = maxMp;
			Sp = sp;
			MaxSp = maxSP;
		}

		public UInt32 Hp { get; private set; }
		public UInt32 MaxHp { get; private set; }
		public UInt32 Mp { get; private set; }
		public UInt32 MaxMp { get; private set; }
		public UInt32 Sp { get; private set; }
		public UInt32 MaxSp { get; private set; }
	}
}
