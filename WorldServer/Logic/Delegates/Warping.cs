using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Logic.Delegates
{
	internal static class Warping
	{
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
			if(character == null || instance == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character or instance is null");
				return;
			}

			switch (npcId)
			{
				case (Byte)SpecialWarpIndex.NPCSIDX_RETN:
				{
					//todo: inventory code
					if (!client.World.InstanceManager.WarpClientReturn(client))
					{
						client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "error while warping (return)");
						return;
					}
					break;
				}
				case (Byte)SpecialWarpIndex.NPCSIDX_GM:
				{
					if(client.Character.Nation != NationCode.NATION_GM)
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
					if(npcId >= 52 && npcId <= 68)
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
