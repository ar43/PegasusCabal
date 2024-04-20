using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.JSON
{
	public class PositionDataRoot
	{
		public PositionDataRoot(PositionDataEntry position)
		{
			Position = position;
		}

		public PositionDataEntry Position {  get; set; }
		
	}

	public class PositionDataEntry
	{
		public PositionDataEntry(UInt16 x, UInt16 y)
		{
			X = x;
			Y = y;
		}

		public UInt16 X { get; set; }
		public UInt16 Y { get; set; }
	}
}
