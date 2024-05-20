namespace LibPegasus.JSON
{
	public class EquipmentDataJSONRoot
	{
		public Dictionary<UInt32, EquipmentDataJSONItem> EquipmentData { get; set; }

		public EquipmentDataJSONRoot(Dictionary<UInt32, EquipmentDataJSONItem> data)
		{
			EquipmentData = data;
		}
	}

	public class EquipmentDataJSONItem
	{
		public UInt32 Kind { get; set; }
		//public UInt32 Option { get; set; }

		public EquipmentDataJSONItem(UInt32 kind)
		{
			Kind = kind;
			//Option = option;
		}
	}
}
