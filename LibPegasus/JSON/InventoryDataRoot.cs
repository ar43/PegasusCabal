namespace LibPegasus.JSON
{
	public class InventoryDataRoot
	{
		public Dictionary<UInt32, InventoryDataItem> InventoryData { get; set; }

		public InventoryDataRoot(Dictionary<UInt32, InventoryDataItem> data)
		{
			InventoryData = data;
		}
	}

	public class InventoryDataItem
	{
		public UInt32 Kind { get; set; }
		public UInt32 Option { get; set; }

		public InventoryDataItem(UInt32 kind, UInt32 option)
		{
			Kind = kind;
			Option = option;
		}
	}
}
