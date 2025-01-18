using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime;
using WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MissionDungeonRuntime
{
	internal enum MissionDungeonStatus
	{
		AWAITING,
		IN_PROGRESS,
		FINISHED
	}
	internal class MissionDungeonManager
	{
		public MissionDungeonDataMain MissionDungeonData { get; private set; }
		private MobManager _mobManager;
		public MissionDungeonStatus MissionDungeonStatus { get; private set; }
		private List<int> _charList;
		private DateTime _startedTime;

		public MissionDungeonManager(MissionDungeonDataMain missionDungeonData, MobManager mobManager)
		{
			MissionDungeonData = missionDungeonData;
			MissionDungeonStatus = MissionDungeonStatus.AWAITING;
			_startedTime = DateTime.MinValue;
			_charList = new List<int>();
			_mobManager = mobManager;
		}

		public int GetDungeonId()
		{
			return MissionDungeonData.MissionDungeonInfo.QDungeonIdx;
		}

		public int GetStartWarpId()
		{
			return MissionDungeonData.MissionDungeonInfo.WarpIdx;
		}

		public int GetTimeLimit()
		{
			return MissionDungeonData.MissionDungeonInfo.MissionTimeout;
		}

		public void RegisterClient(Client c)
		{
			if(MissionDungeonStatus == MissionDungeonStatus.AWAITING && !_charList.Contains(c.Character.Id))
			{
				_charList.Add(c.Character.Id);
			}
			else
			{
				throw new Exception("Client already registered or it is too late");
			}
		}

		private void SpawnStartingMobs()
		{
			foreach(var mobSpawnInfo in MissionDungeonData.MissionDungeonMMap.Values)
			{
				if(mobSpawnInfo.MobSpawnData.SpawnDefault > 0)
				{
					_mobManager.SpawnDungeonMob(mobSpawnInfo);
				}
			}
		}

		private async void ExecuteActGroup(MissionDungeonActGroup actGroup)
		{
			await Task.Delay(actGroup.EvtDelay);

			var action = (TriggerAction)actGroup.TgtAction;

			switch (action)
			{
				case TriggerAction.TA_KILL:
				{
					_mobManager.KillMob(actGroup.TgtMMapIdx);
					break;
				}
				default:
				{
					throw new NotImplementedException();
				}
			}
		}

		private void Trigger(MissionDungeonTrigger trigger)
		{
			var targetActGroupIdx = trigger.EvtActGroupIdx;

			foreach(var actGroup in MissionDungeonData.MissionDungeonActGroup)
			{
				if(actGroup.EvtActGroupIdx == targetActGroupIdx)
				{
					ExecuteActGroup(actGroup);
				}
			}
		}

		public void NpcTrigger(int trgId, int npcIdx)
		{
			var trigger = MissionDungeonData.MissionDungeonTriggers[trgId];
			if (trigger == null)
				throw new Exception("trigger does not exist");
			if (trigger.TrgNpcIdx != npcIdx)
				throw new Exception("npc idx does not match");

			Trigger(trigger);
		}

		internal Int32 StartDungeon(Client client)
		{
			if (MissionDungeonStatus == MissionDungeonStatus.AWAITING && _charList.Contains(client.Character.Id))
			{
				MissionDungeonStatus = MissionDungeonStatus.IN_PROGRESS;
				_startedTime = DateTime.UtcNow;
				SpawnStartingMobs();
				return GetTimeLimit();
			}
			else
			{
				throw new Exception("Client already registered or it is too late");
			}
		}
	}
}
