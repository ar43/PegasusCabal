using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime;
using WorldServer.Logic.WorldRuntime.MissionDungeonDataRuntime;
using WorldServer.Packets.S2C;
using Timer = LibPegasus.Utils.Timer;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MissionDungeonRuntime
{
	internal enum MissionDungeonStatus
	{
		AWAITING,
		IN_PROGRESS,
		FINISHED,
		READY_TO_EXIT,
	}

	internal class PendingDungeonAction
	{
		public MissionDungeonActGroup ActGroup { get; private set; }
		public Timer Timer { get; private set; }	

		public PendingDungeonAction(MissionDungeonActGroup actGroup, Timer timer)
		{
			ActGroup = actGroup;
			Timer = timer;
		}
	}
	internal class MissionDungeonManager
	{
		public MissionDungeonDataMain MissionDungeonData { get; private set; }
		private MobManager _mobManager;
		public MissionDungeonStatus MissionDungeonStatus { get; private set; }
		private List<int> _charList;
		private DateTime _startedTime;
		private List<PendingDungeonAction> _pendingDungeonActions;
		private Dictionary<int, int> _mobDeathCounter;
		private Instance _instance;

		public MissionDungeonManager(MissionDungeonDataMain missionDungeonData, MobManager mobManager, Instance instance)
		{
			MissionDungeonData = missionDungeonData;
			MissionDungeonStatus = MissionDungeonStatus.AWAITING;
			_startedTime = DateTime.MinValue;
			_charList = new List<int>();
			_mobManager = mobManager;
			_pendingDungeonActions = new List<PendingDungeonAction>();
			_mobDeathCounter = new();
			_instance = instance;
		}

		public int GetDungeonId()
		{
			return MissionDungeonData.MissionDungeonInfo.QDungeonIdx;
		}

		public int GetStartWarpId()
		{
			return MissionDungeonData.MissionDungeonInfo.WarpIdx;
		}

		public int GetEndWarpIdForSuccess()
		{
			return MissionDungeonData.MissionDungeonInfo.WarpIdxForSucess;
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

		private void ExecuteActGroup(MissionDungeonActGroup actGroup)
		{
			var action = (TriggerAction)actGroup.TgtAction;

			switch (action)
			{
				case TriggerAction.TA_KILL:
				{
					_mobManager.KillMob(actGroup.TgtMMapIdx);
					break;
				}
				case TriggerAction.TA_DELETE:
				{
					_mobManager.DeleteMob(actGroup.TgtMMapIdx);
					break;
				}
				case TriggerAction.TA_SPAWN:
				{
					_mobManager.SpawnDungeonMob(MissionDungeonData.MissionDungeonMMap[actGroup.TgtMMapIdx]);
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
					var pendingAction = new PendingDungeonAction(actGroup, new Timer(DateTime.UtcNow, (double)actGroup.EvtDelay, false));
					_pendingDungeonActions.Add(pendingAction);
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

		internal void MobTrigger(Int32 trgId)
		{
			var trigger = MissionDungeonData.MissionDungeonTriggers[trgId];
			Trigger(trigger);
		}

		public void OnMobDeath(int speciesIdx)
		{
			_mobDeathCounter.TryAdd(speciesIdx, 0);
			_mobDeathCounter[speciesIdx]++;

			CheckDungeonEnd();
		}

		private void End()
		{
			if (MissionDungeonStatus != MissionDungeonStatus.IN_PROGRESS)
				throw new Exception("unexpected End");
			MissionDungeonStatus = MissionDungeonStatus.FINISHED;

			//TODO: kill all mobs
			

			var nfy = new NFY_QdppComplet(0);
			_instance.Broadcast(nfy); // TODO: questionable
		}

		private void CheckDungeonEnd()
		{
			var missionMobs = MissionDungeonData.MissionDungeonPP.MissionMobs;
			var missionNpc = MissionDungeonData.MissionDungeonPP.MissionNPC;
			if (missionMobs == null || missionMobs.Length > 2)
				throw new NotImplementedException();
			if(missionNpc != 0)
				throw new NotImplementedException();

			var endSpecies = missionMobs[0];
			var endKillCount = missionMobs[1];

			if(_mobDeathCounter.ContainsKey(endSpecies) && _mobDeathCounter[endSpecies] == endKillCount)
			{
				End();
			}
		}

		public void Update()
		{
			foreach(var action in _pendingDungeonActions.ToList())
			{
				if(action.Timer.Tick())
					ExecuteActGroup(action.ActGroup);
				if(action.Timer.Finished)
					_pendingDungeonActions.Remove(action);
			}


			
		}

		internal void RequestFinishDungeon(Client client, UInt16 slotId, Byte success, DungeonEndCause cause, Byte npcId)
		{
			if(success == 1 && cause == DungeonEndCause.Success && MissionDungeonStatus == MissionDungeonStatus.FINISHED)
			{
				MissionDungeonStatus = MissionDungeonStatus.READY_TO_EXIT;
				var rsp = new RSP_QuestDungeonEnd(0, 1);
				client.PacketManager.Send(rsp);

				
				_instance.NotifyAllDungeonEnd(success, cause);
			}
			else
			{
				throw new NotImplementedException("check it out");
			}
		}
	}
}
