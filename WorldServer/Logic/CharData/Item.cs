using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal class Item
	{
		public Item()
		{
			Kind = 0;
			Option = 0;
			Serial = 0;
			Duration = 0;
		}

		public Item(UInt32 kind, UInt32 option)
		{
			Kind = kind;
			Option = option;
			Serial = 0;
			Duration = 0;
		}

		public UInt32 Kind { get; private set; }

		public UInt32 Option { get; private set; } 
		public UInt32 Serial { get; private set; }
		public UInt32 Duration { get; private set; } //change to period

		public void SetKind(UInt32 kind)
		{
			Kind = kind;
		}

		public void SetOption(UInt32 option)
		{
			Option = option;
		}


	}
}
