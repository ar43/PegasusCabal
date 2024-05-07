using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal class LiveStyle
	{
		private UInt32 _value;
		public LiveStyle(UInt32 value) 
		{
			_value = value;
		}
		public UInt32 Serialize()
		{
			//TODO
			return _value;
		}
		public void Set(UInt32 value)
		{
			_value = value;
		}
	}
}
