using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal class Skill
	{
		public Skill(UInt16 id, byte level)
		{
			Id = id;
			Level = level;
		}

		public UInt16 Id { get; private set; }
		public byte Level { get; private set; }

	}
}
