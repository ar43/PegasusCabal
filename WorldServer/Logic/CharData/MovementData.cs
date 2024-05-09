using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData
{
	internal class MovementData
	{
		public bool IsMoving { get; private set; }
		public UInt16 StartX { get; private set; }
		public UInt16 StartY { get; private set; }
		public UInt16 EndX { get; private set; }
		public UInt16 EndY { get; private set; }
		public DateTime StartTime { get; private set; }

		public MovementData(Boolean isMoving, UInt16 startX, UInt16 startY, UInt16 endX, UInt16 endY, DateTime startTime)
		{
			IsMoving = isMoving;
			StartX = startX;
			StartY = startY;
			EndX = endX;
			EndY = endY;
			StartTime = startTime;
		}

		public void Start(UInt16 startX, UInt16 startY, UInt16 endX, UInt16 endY)
		{
			IsMoving = true;
			StartX = startX;
			StartY = startY;
			EndX = endX;
			EndY = endY;
			StartTime = DateTime.Now;
		}

		public void End()
		{
			IsMoving = false;
		}
	}
}
