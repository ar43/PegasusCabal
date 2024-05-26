using Shared.Protos;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncEquipment
	{
		public DbSyncEquipment(EquipmentData equipmentData)
		{
			EquipmentData = equipmentData;
		}

		public EquipmentData EquipmentData { get; private set; }
	}
}
