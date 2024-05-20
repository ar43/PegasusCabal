using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

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
