using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime.LootDataRuntime
{
	internal class OptionPoolFlag
	{
		public OptionPoolFlag(Boolean lR, Boolean lS, Boolean rS, Boolean lRS, Boolean unique)
		{
			LR = lR;
			LS = lS;
			RS = rS;
			LRS = lRS;
			Unique = unique;
		}

		public bool LR {  get; private set; }
		public bool LS { get; private set; }
		public bool RS { get; private set; }
		public bool LRS { get; private set; }
		public bool Unique { get; private set; }
	}
}
