namespace WorldServer.Logic.CharData.Styles.Coefs
{
	internal class StatCoef
	{
		public StatCoef(Int32 sTR, Int32 dEX, Int32 iNT)
		{
			STR = sTR;
			DEX = dEX;
			INT = iNT;
		}

		public StatCoef(int[]? array)
		{
			if (array == null) throw new ArgumentNullException("array");
			STR = array[0];
			DEX = array[1];
			INT = array[2];
		}

		public int STR { get; private set; }
		public int DEX { get; private set; }
		public int INT { get; private set; }
	}
}
