using WorldServer.Logic.CharData.Skills;

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

		public static void Goto(Client client, List<string>? args)
		{
			if (args.Count != 2)
			{
				client.SendServerMessage("Invalid args");
			}
			
		}

		public static void Reset(Client client, List<string>? args)
		{
			if (args.Count != 2)
			{
				client.SendServerMessage("Invalid args");
			}
			if (args.ElementAt(1).ToLower() == "quest")
			{
				client.SendServerMessage("Resetting quests..");
				client.Character.QuestManager.Reset();
			}
			else if (args.ElementAt(1).ToLower() == "lvl")
			{

			}
			else if (args.ElementAt(1).ToLower() == "activequest")
			{
				client.SendServerMessage("Resetting active quest..");
				client.Character.QuestManager.Reset(true);
			}
			else if (args.ElementAt(1).ToLower() == "inv")
			{
				client.SendServerMessage("Resetting inventory..");
				client.Character.Inventory.DebugWipe();
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
				client.Character.Skills.DebugAddSkill(33, new Skill(146, 1));
			}
			else if (args.ElementAt(1).ToLower() == "exp")
			{
				client.SendServerMessage("Giving 70K XP");
				client.Character.Stats.AddExp(70000);
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
				int[] list = { 3001, 3002, 3003, 3005, 3006, 3007, 3008, 3009, 3010, 3011, 3012, 3013, 3014, 3015, 3016, 3049, 3050, 3051, 3052, 3053, 3054, 3055, 3056, 3057, 3058, 3059, 3060, 3159, 3153 };
				client.Character.QuestManager.Reset();
				client.Character.Style.SetMasteryLevel(2);
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
				for (int i = 0; i < cqList.Count; i++)
				{
					if (cqList[i] == true)
						client.SendServerMessage("q" + i.ToString());
				}
			}
			else if (args.ElementAt(1).ToLower() == "exp")
			{
				client.SendServerMessage($"Printing exp: {client.Character.Stats.Exp}");
			}
			else if (args.ElementAt(1).ToLower() == "dungeon")
			{
				client.SendServerMessage($"Printing dungeon: {client.Character.Location.Instance.MissionDungeonManager.GetDungeonId()}");
			}
			else if (args.ElementAt(1).ToLower() == "objective")
			{
				client.SendServerMessage($"Printing stuff: {client.Character.QuestManager.ActiveQuests[0].Flags}/{client.Character.QuestManager.ActiveQuests[0].GetEndFlags()}");
				client.SendServerMessage($"Printing stuff2: {client.Character.QuestManager.ActiveQuests[0].DungeonProgress.Count}/");
				client.SendServerMessage("brkpoint");
			}
			else
			{
				client.SendServerMessage("Invalid args");
			}
		}
	}
}
