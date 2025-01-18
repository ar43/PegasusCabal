using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Warping
	{
		internal static void OnEnterDungeon(Client client, Int32 dungeonId, Int32 warpType, Int32 npcId, Int32 u2, Int32 u3, Int32 mapId)
		{
			var character = client.Character;
			var instance = client.Character.Location.Instance;
			var movement = client.Character.Location.Movement;

			if (character == null || instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character or instance is null");
				return;
			}

			foreach (var quest in character.QuestManager.ActiveQuests.Values)
			{
				if(quest.QuestInfoMain.MissionDungeon != null && quest.QuestInfoMain.MissionDungeon[0] == dungeonId && quest.StoredInstanceId == 0)
				{
					var dungeon = client.World.InstanceManager.AddDungeonInstance((MapId)mapId, dungeonId);

					if (dungeon == null)
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "dungeon does not exist");
						return;
					}

					var id = dungeon.Id;
					quest.StoreInstanceId(id);
					client.Character.Location.PendingDungeon.Set(id, dungeonId);
					client.Character.Location.LastFieldLocInfo = new(movement.X, movement.Y, (int)instance.MapId);

					Debug.Assert(dungeon.MapId == (MapId)mapId);
					

					//TODO stuff with npc

					var rsp = new RSP_EnterDungeon(1, dungeonId, warpType, npcId, u2, u3, (int)dungeon.MapId);
					client.PacketManager.Send(rsp);
					return;
				}
			}

			client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "could not start the requested dungeon");
		}

		internal static void OnWarpCommand(Client client, Byte npcId, UInt16 slot, UInt32 worldType, UInt32 target)
		{
			/*
			#define NPCSIDX_IWAR_FORT   68 
			#define NPCSIDX_IWAR_RELIVE 67  
			#define NPCSIDX_IWAR_WARP   66  
			#define NPCSIDX_IWAR_BF		65	
			#define NPCSIDX_IWAR_LOBBY	64	
			#define NPCSIDX_DEAD		63
			#define NPCSIDX_RETN		62
			#define NPCSIDX_NAVI		61
			#define NPCSIDX_TPNT		60
			#define NPCSIDX_UQDK		59
			#define NPCSIDX_QDWP		58
			#define NPCSIDX_QDEX		57
			#define NPCSIDS_BPNT		56
			#define NPCSIDS_PRSN		55
			#define NPCSIDX_DEAD_PK		54	
			#define NPCSIDX_RSRT		53
			#define NPCSIDX_GM			52
			*/


			var character = client.Character;
			var instance = client.Character.Location.Instance;
			if (character == null || instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character or instance is null");
				return;
			}

			switch (npcId)
			{
				case (Byte)SpecialWarpIndex.NPCSIDX_RETN:
				{
					if (!client.Character.Inventory.UseItem(slot))
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "error while warping (return) - error on item usage");
						return;
					}
					if (!client.World.InstanceManager.WarpClientReturn(client))
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "error while warping (return)");
						return;
					}
					break;
				}
				case (Byte)SpecialWarpIndex.NPCSIDX_QDEX:
				{
					if (!client.World.InstanceManager.WarpClientExitDungeon(client))
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "error while warping (exit dungeon)");
						return;
					}
					break;
				}
				case (Byte)SpecialWarpIndex.NPCSIDX_GM:
				{
					if (!client.isGm())
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "tried to gm warp without gm nation");
						return;
					}
					var targetY = worldType & 0xFF;
					var targetX = (worldType >> 8) & 0xFF;
					if (!client.World.InstanceManager.WarpClientAbsolute(client, (Int32)targetX, (Int32)targetY))
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "gm warp error");
						return;
					}
					break;
				}
				default:
				{
					if (npcId >= 52 && npcId <= 68)
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, $"unimplemented custom npcId {npcId}");
						return;
					}
					if (!client.World.InstanceManager.WarpClientByNpcId(client, npcId))
					{
						//todo: Distance check
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "error while warping (npcId)");
						return;
					}
					break;
				}
			}
		}
	}
}
