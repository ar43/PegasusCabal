using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncInventory
	{
		public InventoryData InventoryData { get; private set; }
		public UInt64 Alz { get; private set; }

		public DbSyncInventory(InventoryData inventoryData, UInt64 alz)
		{
			InventoryData = inventoryData;
			Alz = alz;
		}
	}
}
