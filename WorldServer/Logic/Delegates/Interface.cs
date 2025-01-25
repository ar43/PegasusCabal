using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;
using WorldServer.Logic.CharData;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.Delegates
{
	internal static class Interface
	{
		internal static void OnAutoStat(Client client, Int32 str, Int32 dex, Int32 intelligence)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not yet loaded");
				return;
			}

			var currentStr = client.Character.Stats.Str;
			var currentDex = client.Character.Stats.Dex;
			var currentInt = client.Character.Stats.Int;
			var currentPnt = client.Character.Stats.Pnt;

			if (client.Character.Stats.SpendStatPoint(StatType.STAT_STR, str) &&
				client.Character.Stats.SpendStatPoint(StatType.STAT_DEX, dex) &&
				client.Character.Stats.SpendStatPoint(StatType.STAT_INT, intelligence))
			{
				var rsp = new RSP_AutoStat(str,dex,intelligence);
				client.PacketManager.Send(rsp);
			}
			else
			{
				client.Character.Stats.SetAbsoluteStats(currentStr, currentDex, currentInt, currentPnt);
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "autostat error");
				return;
			}
		}

		internal static void OnPremiumDataRequest(Client client)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not yet loaded");
				return;
			}

			//TEMP TODO
			var premiumServices = new List<PremiumService>();
			premiumServices.Add(new PremiumService());
			premiumServices[0].GPS = 1;
			premiumServices[0].Index = 1;
			premiumServices[0].AtDummy = 1;
			premiumServices[0].EfDummy = 1;
			var packet_duration = new NFY_DurationSvcData(premiumServices);
			client.PacketManager.Send(packet_duration);
		}

		internal static void OnQuickLinkSet(Client client, Int16 quickSlot, Int16 skillSlot)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not yet loaded");
				return;
			}

			try
			{
				client.Character.QuickSlotBar.Set(quickSlot, skillSlot);
			}
			catch (Exception ex)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
				return;
			}

			var rsp = new RSP_QuickLnkSet(1);
			client.PacketManager.Send(rsp);
		}

		internal static void OnQuickLinkSwitch(Client client, Int16 quickSlot1, Int16 quickSlot2)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not yet loaded");
				return;
			}

			try
			{
				client.Character.QuickSlotBar.Swap(quickSlot1, quickSlot2);
			}
			catch (Exception ex)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
				return;
			}

			var rsp = new RSP_QuickLnkSwitch(1);
			client.PacketManager.Send(rsp);
		}

		internal static void OnStatSpend(Client client, Byte stat)
		{
			if (client.Character == null)
			{
				client.Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "character not yet loaded");
				return;
			}

			var statType = (StatType)stat;

			if(client.Character.Stats.SpendStatPoint(statType))
			{
				var rsp = new RSP_UseStatBons(1);
				client.PacketManager.Send(rsp);
			}
			else
			{
				var rsp = new RSP_UseStatBons(0);
				client.PacketManager.Send(rsp);
				Debug.Assert(false);
			}
		}

		internal static void OnUpdateHelpInfo(Client client)
		{
			var packet = new RSP_UpdateHelpInfo(1);
			client.PacketManager.Send(packet);
		}
	}
}
