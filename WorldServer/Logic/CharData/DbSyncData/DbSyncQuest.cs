using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.CharData.DbSyncData
{
	internal class DbSyncQuest
	{
		public DbSyncQuest(ActiveQuestData activeQuestData, CompletedQuestsData completedQuestsData)
		{
			ActiveQuestData = activeQuestData;
			CompletedQuestsData = completedQuestsData;
		}

		public ActiveQuestData ActiveQuestData { get; private set; }
		public CompletedQuestsData CompletedQuestsData { get; private set;}
	}
}
