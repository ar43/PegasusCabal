using System.Diagnostics;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime
{
	internal class AggroTable
	{
		public Client? MaxAggroChar { get; private set; }
		public int MaxAggro { get; private set; }
		public int SecondAggro { get; private set; }
		private int _aggroCounter;

		public void Reset()
		{
			_aggroCounter = 0;
			MaxAggroChar = null;
			MaxAggro = 0;
			SecondAggro = 0;
		}

		public void Add(int aggro, Client newAttacker, Client? currentDefender)
		{
			if (_aggroCounter < 10)
			{
				if (aggro >= MaxAggro)
				{
					MaxAggro = aggro;
					MaxAggroChar = newAttacker;
				}
				else if (aggro >= SecondAggro)
				{
					SecondAggro = aggro;
				}
				_aggroCounter++;
			}
			else
			{
				_aggroCounter = 1;
				if (aggro >= SecondAggro)
				{
					MaxAggroChar = newAttacker;
				}
				else
				{
					//Debug.Assert(currentDefender != null);
					MaxAggroChar = currentDefender;
				}
				MaxAggro = SecondAggro;
				SecondAggro = 0;
			}
		}
	}
}
