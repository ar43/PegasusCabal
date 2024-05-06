using LibPegasus.Enums;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;
using WorldServer.Logic.CharData;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class CharSelect
	{
		internal static async void OnCharacterRequest(Client client)
		{
			var reply = await client.SendCharListRequest();
			var subpassData = await client.GetSubPasswordData();
			if(subpassData.Item1.Length > 0)
			{
				reply.IsPinSet = true;
			}
			else
			{
				reply.IsPinSet = false;
			}
			
			var packet = new RSP_GetMyChartr(reply);
			client.PacketManager.Send(packet);
		}

		internal static void OnGetServerEnv(Client client)
		{
			var packet = new RSP_ServerEnv(ServerConfig.Get().GameSettings);
			client.PacketManager.Send(packet);
		}

		internal static void OnGetSvrTime(Client client)
		{
			var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalSeconds / 60;

			var packet = new RSP_GetSvrTime(time, (short)offset);
			client.PacketManager.Send(packet);
		}

		internal static void OnChargeInfoRequest(Client client)
		{
			var packet = new RSP_ChargeInfo(0, 0, 0);
			client.PacketManager.Send(packet);
		}

		internal static async void OnCreate(Client client, Byte battleStyle, Byte rank, Byte face, Byte hairColor, Byte hairStyle, Byte aura, Byte gender, Byte showHelmet, Boolean joinNoviceGuild, Byte slot, string name)
		{
			var character = new Character(new Style(battleStyle, rank, face, hairColor, hairStyle, aura, gender, showHelmet), name);

			if(character.Verify())
			{
				var reply = await client.SendCharCreationRequest(character, slot);

				var packet = new RSP_NewMyChartr(reply.CharId, (CharCreateResult)reply.Result);
				client.PacketManager.Send(packet);
			}
			else
			{
				var packet = new RSP_NewMyChartr(0, CharCreateResult.DATABRK);
				client.PacketManager.Send(packet);
			}
		}

		internal async static void OnSubpasswordCheckRequest(Client client)
		{
			var subpassData = await client.GetSubPasswordData();
			
			if(subpassData.Item1 == "")
			{
				var packet = new RSP_SubPasswordCheckRequest(false);
				client.PacketManager.Send(packet);
			}
			else
			{

				if(subpassData.Item2 == null)
				{
					var packet = new RSP_SubPasswordCheckRequest(true);
					client.PacketManager.Send(packet);
				}
				else
				{
					var result = (DateTime.UtcNow - subpassData.Item2).Value.TotalHours;
					if(result > 3) //TODO: get a db value of this, not just constant 3
					{
						var packet = new RSP_SubPasswordCheckRequest(true);
						client.PacketManager.Send(packet);
					}
					else
					{
						var packet = new RSP_SubPasswordCheckRequest(false);
						client.PacketManager.Send(packet);
						client.ConnectionInfo.SubPasswordAuthenticated = true;
					}
				}
			}
			
		}

		internal async static void OnSubPasswordSet(Client client, String subpass, SubPasswordType subpassType, UInt32 secretQuestion, String secretAnswer, SubPasswordLockType subpassLockType)
		{
			if(subpassType == SubPasswordType.LOGIN && subpassLockType == SubPasswordLockType.UNLOCKED)
			{
				var attempt = await client.SetSubPassword(subpass);

				if(attempt == true)
				{
					var packet = new RSP_SubPasswordSet(1, 0, SubPasswordType.LOGIN, SubPasswordLockType.UNLOCKED);
					client.PacketManager.Send(packet);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		internal static async void OnSubPasswordCheck(Client client, String subpass, SubPasswordType subpassType, Byte hours, SubPasswordLockType subpassLockType)
		{
			var subpassData = await client.GetSubPasswordData();

			if(subpassData.Item1 == subpass)
			{
				var packet = new RSP_SubPasswordCheck(1, 0, subpassType, subpassLockType);
				client.PacketManager.Send(packet);
				await client.SetSubPasswordVerificationDate();
				client.ConnectionInfo.SubPasswordAuthenticated = true;
			}
			else
			{
				var packet = new RSP_SubPasswordCheck(0, 1, subpassType, subpassLockType);
				client.PacketManager.Send(packet);
			}
		}

		internal static async void OnInitialize(Client client, UInt32 charId)
		{
			if(client.Character != null)
			{
				client.Disconnect("trying to init while already being init", ConnState.ERROR);
				return;
			}

			if(!client.ConnectionInfo.SubPasswordAuthenticated)
			{
				client.Disconnect("trying to init without being subpass authenticated", ConnState.ERROR);
				return;
			}

			var charToAccId = charId / 8;
			if(charToAccId != client.ConnectionInfo.AccountId)
			{
				client.Disconnect("invalid char id", ConnState.ERROR);
				return;
			}

			var character = await client.LoadCharacter(charId);
			client.Character = character;
			if(client.Character != null)
			{
				var packet = new NFY_ChargeInfo(0, 0, 0); //TODO: premium service
				client.PacketManager.Send(packet);

				var packet_init = new RSP_Initialized(client.Character, 0, client.ConnectionInfo.UserId);
				client.PacketManager.Send(packet_init);

				var packet_bang = new NFY_PcBangAlert(0, 0);
				client.PacketManager.Send(packet_bang);

				var packet_periodical = new NFY_PeriodicalService(0);
				client.PacketManager.Send(packet_periodical);

				var packet_war = new NFY_LastNationRewardWarResults(0, 0, 0, 0, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch, 0); //TODO: war rewards
				client.PacketManager.Send(packet_war);

				var packet_994 = new NFY_Unk994();
				client.PacketManager.Send(packet_994);

				client.Account = new Account(client.ConnectionInfo.AccountId); //TODO: actually load the account data
			}
			else
			{
				client.Disconnect("char with specified id does not exist", ConnState.ERROR);
				return;
			}
		}
	}
}
