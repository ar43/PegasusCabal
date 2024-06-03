using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.Styles.Coefs
{
	internal class StyleCoef
	{
		public StyleCoef(Int32 a, Int32 b)
		{
			A = a;
			B = b;
		}

		public StyleCoef(int[]? array)
		{
			if(array == null) throw new ArgumentNullException("array");

			A = array[0];
			B = array[1];
		}

		public int A { get; private set; }
		public int B { get; private set; }
	}
}
