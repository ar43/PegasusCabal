using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.CharData.Skills;
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
			CommandList["give"] = CommandDelegates.Give;
			CommandList["print"] = CommandDelegates.Print;
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
			else if (args.ElementAt(1).ToLower() == "lvl")
			{
				
			}
			else
			{
				client.SendServerMessage("Invalid args");
			}
		}

		public static void Give(Client client, List<string>? args)
		{
			if (args.Count != 2)
			{
				client.SendServerMessage("Invalid args");
			}
			if (args.ElementAt(1).ToLower() == "gmskill")
			{
				client.SendServerMessage("Gave GM skill..");
				client.Character.Skills.DebugAddSkill(32, new Skill(147, 1));
				client.Character.Skills.DebugAddSkill(33, new Skill(146,1));
			}
			else if (args.ElementAt(1).ToLower() == "exp")
			{
				client.SendServerMessage("Giving 10K XP");
				client.Character.Stats.AddExp(10000);
			}
			else if (args.ElementAt(1).ToLower() == "qdbg0")
			{
				client.SendServerMessage("Setting quests to specific setup..");
				int[] list = { 3001, 3002, 3003, 3005, 3006, 3007, 3008, 3009 };
				client.Character.QuestManager.Reset();
				foreach (var qnum in list)
				{
					client.Character.QuestManager.CompletedQuests[qnum] = true;
				}
			}
			else if (args.ElementAt(1).ToLower() == "qdbg1")
			{
				client.SendServerMessage("Setting quests to specific setup..");
				int[] list = { 3001, 3002, 3003, 3005, 3006, 3007, 3008};
				client.Character.QuestManager.Reset();
				foreach (var qnum in list)
				{
					client.Character.QuestManager.CompletedQuests[qnum] = true;
				}
			}
			else
			{
				client.SendServerMessage("Invalid args");
			}
		}

		public static void Print(Client client, List<string>? args)
		{
			
			if (args.Count != 2)
			{
				client.SendServerMessage("Invalid args");
			}
			if (args.ElementAt(1).ToLower() == "cq")
			{
				var cqList = client.Character.QuestManager.CompletedQuests;

				client.SendServerMessage("Printing completed quests:");
				for(int i = 0; i < cqList.Count; i++)
				{
					if (cqList[i] == true)
						client.SendServerMessage("q" + i.ToString());
				}
			}
			else if (args.ElementAt(1).ToLower() == "exp")
			{
				client.SendServerMessage($"Printing exp: {client.Character.Stats.Exp}");
			}
			else
			{
				client.SendServerMessage("Invalid args");
			}
		}
	}
}
