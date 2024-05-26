namespace WorldServer.Logic.CharData
{
	internal class BuffFlag
	{
		private UInt32 _value;

		public BuffFlag(UInt32 value)
		{
			_value = value;
		}

		public UInt32 Serialize()
		{
			return _value;
		}

		public void Set(UInt32 value)
		{
			_value = value;
		}
	}
}
