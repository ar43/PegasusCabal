namespace LibPegasus.JSON
{
	public class QuickSlotDataRoot
	{
		public QuickSlotDataRoot(Dictionary<UInt16, QuickSlotDataEntry> quickSlotData)
		{
			QuickSlotData = quickSlotData;
		}

		public Dictionary<UInt16, QuickSlotDataEntry> QuickSlotData { get; set; }

	}

	public class QuickSlotDataEntry
	{
		public QuickSlotDataEntry(UInt16 id)
		{
			Id = id;
		}

		public UInt16 Id { get; set; }
	}
}
