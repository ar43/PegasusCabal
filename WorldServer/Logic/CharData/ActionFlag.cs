using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal class ActionFlag
	{
		private UInt16 _value;

		public ActionFlag(UInt16 value)
		{
			this._value = value;
		}

		public UInt16 Serialize()
		{
			return _value;
		}

		public void Set(UInt16 value)
		{
			_value = value;
		}
	}
}
