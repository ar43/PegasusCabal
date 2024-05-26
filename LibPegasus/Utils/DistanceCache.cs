namespace LibPegasus.Utils
{
	public class DistanceCache
	{
		public static readonly int TABLE_LENGTH = 64;
		public static DistanceCache Get => _instance.Value;

		private static readonly Lazy<DistanceCache> _instance =
			new Lazy<DistanceCache>(() => new DistanceCache());

		private readonly float[,] _distanceTable = new float[TABLE_LENGTH, TABLE_LENGTH];

		public DistanceCache()
		{
			for (int i = 0; i < TABLE_LENGTH; i++)
			{
				for (int j = 0; j < TABLE_LENGTH; j++)
				{
					_distanceTable[j, i] = (float)Math.Sqrt((float)j * (float)j + (float)i * (float)i);
				}
			}
		}

		public float GetDistance(int x, int y)
		{
			return _distanceTable[x, y];
		}
	}
}
