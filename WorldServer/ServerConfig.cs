using System.Text.Json;

namespace WorldServer
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public sealed class ServerConfig
	{
		private ServerConfig() { }
		private static Config _instance;

		public static Config Get(string configName)
		{
			if (_instance == null)
			{
				string workingDirectory = Environment.CurrentDirectory;
				string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

				string configFile = projectDirectory + "\\" + configName + ".json";
				string jsonString = File.ReadAllText(configFile);

				_instance = JsonSerializer.Deserialize<Config>(jsonString)!;
			}
			return _instance;
		}
		public static Config Get()
		{
			if (_instance == null)
			{
				throw new Exception("WorldServer ServerConfig error");
			}
			return _instance;
		}
	}


	public class Config
	{
		public Generalsettings GeneralSettings { get; set; }
		public Connectionsettings ConnectionSettings { get; set; }
		public Databasesettings DatabaseSettings { get; set; }
		public Gamesettings GameSettings { get; set; }
	}

	public class Generalsettings
	{
		public int ServerId { get; set; }
		public int ChannelId { get; set; }
		public long ClientMagicKey { get; set; }
	}

	public class Connectionsettings
	{
		public int Port { get; set; }
	}

	public class Databasesettings
	{
		public string ConnString { get; set; }
		public bool EnableDbSync { get; set; }
	}

	public class Gamesettings
	{
		public int MaxLevel { get; set; }
		public bool DummyEnabled { get; set; }
		public bool CashShopEnabled { get; set; }
		public bool NetcafePointsEnabled { get; set; }
		public int MaxRank { get; set; }
		public int LimitLoudCharLv { get; set; }
		public int LimitLoudMasteryLv { get; set; }
		public long LimitInvAlzSave { get; set; }
		public long LimitWhAlzSave { get; set; }
		public long LimitTradeAlz { get; set; }
		public bool AllowDuplicatedPCBangPremium { get; set; }
		public bool GuildBoardEnabled { get; set; }
		public int PCBangPremiumPrioType { get; set; }
		public int UseTradeChannelRestriction { get; set; }
		public bool AgentShopEnabled { get; set; }
		public int UseLordBroadCastCoolTimeSec { get; set; }
		public int DummyLimitLv { get; set; }
		public int AgentShopRestrictionLv { get; set; }
		public int PersonalShopRestrictionLv { get; set; }
		public bool UseTPoint { get; set; }
		public bool UseGuildExpansion { get; set; }
		public bool IgnorePartyInviteDistance { get; set; }
		public bool LimitedBroadCastByLord { get; set; }
		public int LimitNormalChatLev { get; set; }
		public int LimitTradeChatLev { get; set; }
		public int MaxDPLimit { get; set; }
	}




#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
