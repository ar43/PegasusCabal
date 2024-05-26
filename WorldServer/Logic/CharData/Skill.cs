namespace WorldServer.Logic.CharData
{
	internal class Skill
	{
		public Skill(UInt16 id, byte level)
		{
			Id = id;
			Level = level;
		}

		public UInt16 Id { get; private set; }
		public byte Level { get; private set; }

	}
}
