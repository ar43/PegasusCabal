using Shared.Protos;

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
		public CompletedQuestsData CompletedQuestsData { get; private set; }
	}
}
