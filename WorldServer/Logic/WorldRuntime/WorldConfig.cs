using LibPegasus.Parsers.Scp;
using System.Text.RegularExpressions;
using WorldServer.Logic.CharData.Items;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.CharData.Styles;

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

			Item.LoadConfigs(this);
			Skill.LoadConfigs(this);
			Style.LoadConfigs(this);
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
