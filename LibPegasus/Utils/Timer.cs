using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Utils
{
	public class Timer
	{
		private DateTime _startTime;
		private double _interval;
		private bool _autoRestart;

		public Timer(DateTime startTime, Double interval, Boolean autoRestart)
		{
			_startTime = startTime;
			_interval = interval;
			_autoRestart = autoRestart;
		}

		public void SetInterval(double interval)
		{
			_interval = interval;
		}

		public bool Tick()
		{
			var time = DateTime.UtcNow;

			if (time.Ticks - _startTime.Ticks >= TimeSpan.FromSeconds(_interval).Ticks)
			{
				if(_autoRestart)
				{
					_startTime = DateTime.UtcNow;
				}
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
