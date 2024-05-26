using Shared.Protos;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncQuickSlotBar
	{
		public DbSyncQuickSlotBar(QuickSlotData quickSlotData)
		{
			QuickSlotData = quickSlotData;
		}

		public QuickSlotData QuickSlotData { get; private set; }
	}
}
