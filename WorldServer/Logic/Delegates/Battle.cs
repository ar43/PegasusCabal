using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Packets.C2S.PacketSpecificData;

namespace WorldServer.Logic.Delegates
{
	internal static class Battle
	{
		internal static void OnSkillToMobs(Client client, UInt16 skillId, Byte slot, UInt32 u0, UInt16 x, UInt16 y, Byte u1, UInt32 u2, List<MobTarget> mobs)
		{
			if(client.Character == null || client.Character.Skills == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			if(!client.Character.Skills.HasSkill(slot,skillId))
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "skill not learned");
				return;
			}

			var result = client.Character.Location.Instance.OnUserSkillAttacksMob(client, mobs, x, y, slot);

			if(result)
			{
				//... sync?
			}
			else
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "error in OnUserSkillAttacksMob");
				return;
			}
		}
	}
}
