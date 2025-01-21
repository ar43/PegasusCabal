using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Dungeoneering
	{
		internal static void OnDungeonEnd(Client client, UInt16 slotId, Byte success, DungeonEndCause cause, Byte npcId)
		{
			var character = client.Character;
			var instance = client.Character.Location.Instance;
			if (character == null || instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character or instance is null");
				return;
			}

			if (instance.MissionDungeonManager == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not in dungeon");
				return;
			}

			if (slotId != 0xFFFF || success != 1 || cause != DungeonEndCause.Success)
			{
				throw new NotImplementedException();
			}

			try
			{
				instance.MissionDungeonManager.RequestFinishDungeon(client, slotId, success, cause, npcId);
			}
			catch (Exception e)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
				return;
			}
		}

		internal static void OnDungeonMobsActiveRequest(Client client, Byte active)
		{
			var character = client.Character;
			var instance = client.Character.Location.Instance;
			if (character == null || instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character or instance is null");
				return;
			}

			if (instance.MissionDungeonManager == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not in dungeon");
				return;
			}

			if(active == 0)
			{
				//TODO investigate
				return;
			}

			int timeLimit = 0;

			try
			{
				timeLimit = instance.MissionDungeonManager.StartDungeon(client);
			}
			catch (Exception e)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
				return;
			}

			var rsp = new NFY_QuestDungeonStart(timeLimit*1000,timeLimit*1000, 0);
			client.BroadcastNearby(rsp);
		}

		internal static void OnDungeonStart(Client client)
		{
			var character = client.Character;
			var instance = client.Character.Location.Instance;
			if (character == null || instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character or instance is null");
				return;
			}

			if (instance.MissionDungeonManager == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not in dungeon");
				return;
			}

			try
			{
				instance.MissionDungeonManager.RegisterClient(client);
			}
			catch (Exception e)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
				return;
			}

			var rsp = new RSP_DungeonStart(0);
			client.PacketManager.Send(rsp);
		}
	}
}
