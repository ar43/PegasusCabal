using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorldServer.Logic.Extra
{
	internal static class CommandManager
	{
		public static Dictionary<string, Action<Client, List<string>?>> CommandList = new();

		public static void Init()
		{
			CommandList.Clear();

			CommandList["reset"] = CommandDelegates.Reset;
			CommandList["kickme"] = CommandDelegates.KickMe;
			CommandList["testmsg"] = CommandDelegates.TestMsg;
		}
	}

	internal static class CommandDelegates
	{
		public static void KickMe(Client client, List<string>? args)
		{
			client.Disconnect("used kickme cmd", Enums.ConnState.KICKED);
		}

		public static void TestMsg(Client client, List<string>? args)
		{
			Serilog.Log.Debug("hello world");
			client.SendServerMessage("hello world");
		}

		public static void Reset(Client client, List<string>? args)
		{
			if(args.Count != 2)
			{
				client.SendServerMessage("Invalid args");
			}
			if(args.ElementAt(1).ToLower() == "quest")
			{
				client.SendServerMessage("Resetting quests..");
				client.Character.QuestManager.Reset();
			}
			else
			{
				client.SendServerMessage("Invalid args");
			}
		}
	}
}
