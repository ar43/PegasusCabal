using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		public MissionDungeonStatus MissionDungeonStatus { get; private set; }
		private List<int> _charList;

		public MissionDungeonManager(MissionDungeonDataMain missionDungeonData)
		{
			MissionDungeonData = missionDungeonData;
			MissionDungeonStatus = MissionDungeonStatus.AWAITING;
			_charList = new List<int>();
		}

		public int GetDungeonId()
		{
			return MissionDungeonData.MissionDungeonInfo.QDungeonIdx;
		}

		public int GetStartWarpId()
		{
			return MissionDungeonData.MissionDungeonInfo.WarpIdx;
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
	}
}
