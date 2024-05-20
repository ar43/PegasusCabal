namespace LibPegasus.JSON
{
	public class InventoryDataJSONRoot
	{
		public Dictionary<UInt32, InventoryDataJSONItem> InventoryData { get; set; }

		public InventoryDataJSONRoot(Dictionary<UInt32, InventoryDataJSONItem> data)
		{
			InventoryData = data;
		}
	}

	public class InventoryDataJSONItem
	{
		public UInt32 Kind { get; set; }
		public UInt32 Option { get; set; }

		public InventoryDataJSONItem(UInt32 kind, UInt32 option)
		{
			Kind = kind;
			Option = option;
		}
	}
}
