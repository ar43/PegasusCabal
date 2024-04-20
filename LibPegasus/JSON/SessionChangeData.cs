namespace LibPegasus.JSON
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public class SessionChangeData
	{
		public string table { get; set; }
		public string action { get; set; }
		public Data data { get; set; }
	}

	public class Data
	{
		public int auth_key { get; set; }
		public int user_id { get; set; }
		public int channel_id { get; set; }
		public int server_id { get; set; }
		public int account_id { get; set; }
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
