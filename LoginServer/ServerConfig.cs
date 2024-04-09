using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LoginServer
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public sealed class ServerConfig
	{
		private ServerConfig() { }
		private static Config _instance;

		public static Config Get()
		{
			if (_instance == null)
			{
				string workingDirectory = Environment.CurrentDirectory;
				string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

				string configFile = projectDirectory + "\\config.json";
				string jsonString = File.ReadAllText(configFile);

				_instance = JsonSerializer.Deserialize<Config>(jsonString)!;
			}
			return _instance;
		}
	}
	public class Config
	{
		public Generalsettings GeneralSettings { get; set; }
		public Connectionsettings ConnectionSettings { get; set; }
		public Databasesettings DatabaseSettings { get; set; }
	}

	public class Generalsettings
	{
		public bool VerifyClientVersion { get; set; }
		public int ClientVersion { get; set; }
	}

	public class Connectionsettings
	{
		public int Port { get; set; }
	}

	public class Databasesettings
	{
		public string ConnString { get; set; }
	}


#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
