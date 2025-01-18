namespace LibPegasus.Utils
{
	public class Timer
	{
		private DateTime _startTime;
		private double _interval;
		private bool _autoRestart;
		public bool Finished { get; private set; }

		public Timer(DateTime startTime, Double interval, Boolean autoRestart)
		{
			_startTime = startTime;
			_interval = interval;
			_autoRestart = autoRestart;
			Finished = false;
		}

		public void SetInterval(double interval)
		{
			_interval = interval;
		}

		public bool Tick()
		{
			var time = DateTime.UtcNow;

			if (time.Ticks - _startTime.Ticks >= TimeSpan.FromMilliseconds(_interval).Ticks && !Finished)
			{
				if (_autoRestart)
				{
					_startTime = DateTime.UtcNow;
				}
				else
				{
					Finished = true;
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
