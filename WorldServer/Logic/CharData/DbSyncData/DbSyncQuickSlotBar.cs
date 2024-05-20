using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

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
