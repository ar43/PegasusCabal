using LibPegasus.Parsers.Scp;
using System.Text.RegularExpressions;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.CharData.Quests;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.CharData.Styles;
using WorldServer.Logic.Extra;
using WorldServer.Logic.WorldRuntime.LootDataRuntime;

namespace WorldServer.Logic.WorldRuntime
{
	internal class WorldConfig
	{

		public WorldConfig()
		{
			_config = [];
			string workingDirectory = Environment.CurrentDirectory;
			string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Warp.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\NPCShop.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Item.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Mobs.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Skill.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Rank.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Quest.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\ItemReward.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Level.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\MissionDungeon.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\World_drop.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\OptionPool.scp");
			ScpParser.Parse(_config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_Custom\\QuestToDungeon.scp");

			Item.LoadConfigs(this);
			Item.LoadItemRewards(this);
			Skill.LoadConfigs(this);
			Style.LoadConfigs(this);
			Quest.LoadConfigs(this);
			Loot.LoadConfig(this);
			Stats.LoadExpTable(this);

			CommandManager.Init();
		}

		private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _config;

		public Dictionary<string, Dictionary<string, string>> GetConfig(string section)
		{
			if (_config.TryGetValue(section, out var cfg))
			{
				return cfg;
			}
			else
			{
				if (!Regex.IsMatch(section, @"\d+"))
					throw new Exception($"cant find section {section}");
				var mapId = Regex.Match(section, @"\d+").Value;
				string workingDirectory = Environment.CurrentDirectory;
				string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
				var terrainFile = $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_World\\world{mapId}-terrain.scp";
				var npcFile = $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_World\\world{mapId}-npc.scp";
				var mmapFile = $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_World\\world{mapId}-mmap.scp";
				ScpParser.Parse(_config, terrainFile);
				ScpParser.Parse(_config, npcFile);
				ScpParser.Parse(_config, mmapFile);
				if (_config.TryGetValue(section, out var cfg_created))
				{
					return cfg_created;
				}
				else
				{
					throw new Exception("Still cant find section");
				}
			}
		}
	}
}
