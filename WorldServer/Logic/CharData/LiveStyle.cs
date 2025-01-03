namespace WorldServer.Logic.CharData
{
	internal class LiveStyle
	{
		private UInt32 _value;
		public LiveStyle(UInt32 value)
		{
			_value = value;
		}
		public UInt32 Serialize()
		{
			//TODO
			return _value;
		}
		public Byte SerializeByte()
		{
			//TODO
			return (Byte)(_value & 0xFF);
		}
		public void Set(UInt32 value)
		{
			_value = value;
		}
	}
}
