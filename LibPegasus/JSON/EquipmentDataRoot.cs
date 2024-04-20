namespace LibPegasus.JSON
{
	public class EquipmentDataRoot
	{
		public Dictionary<UInt32, EquipmentDataItem> EquipmentData { get; set; }

		public EquipmentDataRoot(Dictionary<UInt32, EquipmentDataItem> data)
		{
			EquipmentData = data;
		}
	}

	public class EquipmentDataItem
	{
		public UInt32 Kind { get; set; }
		//public UInt32 Option { get; set; }

		public EquipmentDataItem(UInt32 kind)
		{
			Kind = kind;
			//Option = option;
		}
	}
}
