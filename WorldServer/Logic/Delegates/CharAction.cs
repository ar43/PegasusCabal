using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.CharData.Styles;
using WorldServer.Packets.C2S.PacketSpecificData;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class CharAction
	{
		private static bool VerifySkill(Character character, int skillId, int slot)
		{
			if (character == null)
				return false;

			if(!character.Skills.HasSkill((UInt16)slot, (UInt16)skillId))
				return false;

			var skill = character.Skills.Get(slot);
			if (skill == null)
				return false;

			if (skill.Id != skillId)
				return false;

			if(skill.GetExclusive() != 0)
			{
				if(skill.GetExclusive() != character.Style.BattleStyleNum)
					return false;
			}

			if (!skill.CheckUseCase(SkillUseCase.ENABLE_NORMAL))
				throw new NotImplementedException();

			if (skill.CheckUseCase(SkillUseCase.ENABLE_ASTRAL) && character.Style.StyleEx.AstralWeapon == 0)
				return false;

			var item_left = character.Equipment.GetItem(Enums.EquipmentIndex.LEFTHAND);
			var item_right = character.Equipment.GetItem(Enums.EquipmentIndex.RIGHTHAND);

			if (skill.CheckUseCase(SkillUseCase.ENABLE_BM1_2HND) || skill.CheckUseCase(SkillUseCase.ENABLE_BM2_2HND))
			{
				if (item_right?.GetItemType() == ItemType.IDT_2HND)
					return true;
			}

			if (skill.CheckUseCase(SkillUseCase.ENABLE_BM1_DUAL) || skill.CheckUseCase(SkillUseCase.ENABLE_BM2_DUAL))
			{
				if (item_left?.GetItemType() == ItemType.IDT_1HND && item_right?.GetItemType() == ItemType.IDT_1HND)
					return true;
			}

			if (skill.CheckUseCase(SkillUseCase.ENABLE_BM1_MAIC) || skill.CheckUseCase(SkillUseCase.ENABLE_BM2_MAIC) ||
				skill.CheckUseCase(SkillUseCase.ENABLE_BM1_MARR) || skill.CheckUseCase(SkillUseCase.ENABLE_BM2_MARR))
			{
				if (item_left?.GetItemType() == ItemType.IDT_MWPN && item_right?.GetItemType() == ItemType.IDT_MWPN)
					return true;
				else if (item_left == null && item_right?.GetItemType() == ItemType.IDT_MWPN)
					return true;
				else if (item_right == null && item_left?.GetItemType() == ItemType.IDT_MWPN)
					return true;
				else if (item_right == null && item_left == null)
					return true;
			}

			if (skill.CheckUseCase(SkillUseCase.ENABLE_BM1_SSHD) || skill.CheckUseCase(SkillUseCase.ENABLE_BM1_SSHD) ||
				skill.CheckUseCase(SkillUseCase.ENABLE_BM1_MSWD) || skill.CheckUseCase(SkillUseCase.ENABLE_BM2_MSWD))
			{
				if (item_right?.GetItemType() == ItemType.IDT_1HND && item_left?.GetItemType() == ItemType.IDT_MWPN)
					return true;
				if (item_right?.GetItemType() == ItemType.IDT_1HND && item_left == null)
					return true;
			}


			return false;
		}
		internal static void OnSkillToMobs(Client client, UInt16 skillId, Byte slot, UInt32 u0, UInt16 x, UInt16 y, Byte u1, UInt32 u2, List<MobTarget> mobs)
		{
			if (client.Character == null || client.Character.Skills == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "null Character");
				return;
			}

			if (!VerifySkill(client.Character, skillId, slot))
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "skill info mismatch");
				return;
			}

			var result = client.Character.Location.Instance.OnUserSkillAttacksMob(client, mobs, x, y, slot);

			if (result)
			{
				//... sync?
			}
			else
			{
				//client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "error in OnUserSkillAttacksMob");
				Serilog.Log.Warning("Error in OnUserSkillAttacksMob");
				return;
			}
		}
		internal static void OnChangeStyle(Client client, Style newStyle, LiveStyle newLiveStyle, BuffFlag newBuffFlag, ActionFlag newActionFlag)
		{
			if (client.Character == null)
			{
				client.Disconnect("character not yet loaded", Enums.ConnState.ERROR);
				return;
			}

			byte toggleHelmet = newStyle.ShowHelmet;
			client.Character.Style.ToggleHelmet(toggleHelmet);

			//TODO: Verification
			client.Character.LiveStyle.Set(newLiveStyle.Serialize());
			client.Character.BuffManager.BuffFlag.Set(newBuffFlag.Serialize());
			client.Character.ActionFlag.Set(newActionFlag.Serialize());

			var packet_rsp = new RSP_ChangeStyle(1); //TODO: send 0 if bad
			client.PacketManager.Send(packet_rsp);

			var packet_nfy = new NFY_ChangeStyle(client.Character);
			client.BroadcastNearby(packet_nfy);
		}

		internal static void OnSkillToUserG31(Client client, UInt16 skillId, Byte slot, UInt16 state, UInt32 u0)
		{
			if (client.Character == null)
			{
				client.Disconnect("character not yet loaded", Enums.ConnState.ERROR);
				return;
			}

			if(!VerifySkill(client.Character, skillId, slot))
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "skill info mismatch");
				return;
			}

			if(state > 1 || state < 0)
			{
				client.Disconnect("invalid state", Enums.ConnState.ERROR);
				return;
			}

			//u0 - Skip anim?

			switch(skillId)
			{
				case 331: //Astral bow
				{
					var item_left = client.Character.Equipment.GetItem(Enums.EquipmentIndex.LEFTHAND);
					var item_right = client.Character.Equipment.GetItem(Enums.EquipmentIndex.RIGHTHAND);
					int grade = 0;

					if(item_left != null)
					{
						if(item_left.GetItemType() != Enums.ItemType.IDT_MWPN)
						{
							client.Disconnect("astral bow with a non-magic weapon", Enums.ConnState.ERROR);
							return;
						}
					}

					if (item_right != null)
					{
						if (item_right.GetItemType() != Enums.ItemType.IDT_MWPN)
						{
							client.Disconnect("astral bow with a non-magic weapon", Enums.ConnState.ERROR);
							return;
						}
					}

					if(item_left == null)
					{
						grade = 0;
					}
					else
					{
						grade = item_left.GetGrade();
					}

					Skill skill = new(skillId, (Byte)grade);
					if(state == 1)
					{
						client.Character.Style.StyleEx.ToggleAstralWeapon(true);
						client.Character.BuffManager.ActivateBuff(skill);
					}
					else
					{
						client.Character.Style.StyleEx.ToggleAstralWeapon(false);
						client.Character.BuffManager.RemoveBuff(skill.Id);
					}
					break;
				}
				case 332: //Astral shield
				{
					var item_left = client.Character.Equipment.GetItem(Enums.EquipmentIndex.LEFTHAND);
					var item_right = client.Character.Equipment.GetItem(Enums.EquipmentIndex.RIGHTHAND);
					int grade = 0;

					if (item_left != null)
					{
						if (item_left.GetItemType() != Enums.ItemType.IDT_MWPN)
						{
							client.Disconnect("astral shield with a non-magic weapon", Enums.ConnState.ERROR);
							return;
						}
					}

					if (item_right == null)
					{
						client.Disconnect("astral shield with empty right hand", Enums.ConnState.ERROR);
						return;
					}

					if (item_left == null)
					{
						grade = 0;
					}
					else
					{
						grade = item_left.GetGrade();
					}

					Skill skill = new(skillId, (Byte)grade);
					if (state == 1)
					{
						client.Character.Style.StyleEx.ToggleAstralWeapon(true);
						client.Character.BuffManager.ActivateBuff(skill);
					}
					else
					{
						client.Character.Style.StyleEx.ToggleAstralWeapon(false);
						client.Character.BuffManager.RemoveBuff(skill.Id);
					}
					break;
				}
				default:
				{
					Serilog.Log.Warning($"unsupported skill used {skillId}");
					return;
				}
			}

			var packet_rsp = new RSP_SkillToUser(skillId, (UInt16)client.Character.Status.Mp, state, u0);
			client.PacketManager.Send(packet_rsp);

			var packet_nfy = new NFY_SkillToUser(skillId, (UInt32)client.Character.Id, client.Character.Style, client.Character.LiveStyle, state, u0);
			client.BroadcastNearby(packet_nfy);

		}
	}
}
