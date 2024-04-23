using LibPegasus.Enums;
using WorldServer.Logic.Char;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class CharSelect
	{
		internal static void OnCharacterRequest(Client client)
		{
			var packet = new RSP_GetMyChartr();
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

		internal static void OnCreate(Client client, Byte battleStyle, Byte rank, Byte face, Byte hairColor, Byte hairStyle, Byte aura, Byte gender, Byte showHelmet, Boolean joinNoviceGuild, Byte slot, string name)
		{
			var character = new Character(new Style(battleStyle, rank, face, hairColor, hairStyle, aura, gender, showHelmet), name);

			if(character.Verify())
			{
				var reply = client.SendCharCreationRequest(character, slot);

				var packet = new RSP_NewMyChartr(reply.Result.CharId, (CharCreateResult)reply.Result.Result);
				client.PacketManager.Send(packet);
			}
			else
			{
				var packet = new RSP_NewMyChartr(0, CharCreateResult.DATABRK);
				client.PacketManager.Send(packet);
			}
		}
	}
}
