using LibPegasus.Parsers.Scp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime
{
	internal class WorldConfig
	{
		public WorldConfig()
		{
			Config = [];
			string workingDirectory = Environment.CurrentDirectory;
			string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
			ScpParser.Parse(Config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Warp.scp");
			ScpParser.Parse(Config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_World\\world1-terrain.scp");
			ScpParser.Parse(Config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_World\\world2-terrain.scp");
			ScpParser.Parse(Config, $"{projectDirectory}\\LibPegasus\\Data\\Raw\\Data\\Data_World\\world3-terrain.scp");
		}

		public Dictionary<string, Dictionary<string, Dictionary<string, string>>> Config { get; private set; }
	}
}
