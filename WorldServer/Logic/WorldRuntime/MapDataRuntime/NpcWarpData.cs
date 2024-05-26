namespace WorldServer.Logic.WorldRuntime.MapDataRuntime
{
	internal class NpcWarpData
	{
		public NpcWarpData(Int32 setIdx, Int32 level, Int32 fee, Int32 type, int targetId)
		{
			SetIdx = setIdx;
			Level = level;
			Fee = fee;
			Type = type;
			TargetId = targetId;
		}

		public int SetIdx { get; private set; }
		public int TargetId { get; private set; }
		public int Level { get; private set; }
		public int Fee { get; private set; }
		public int Type { get; private set; }
	}
}
