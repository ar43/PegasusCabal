namespace WorldServer
{
	internal class ConnectionInfo
	{
		public ConnectionInfo(UInt16 userId, UInt32 authKey)
		{
			UserId = userId;
			AuthKey = authKey;
		}

		public UInt16 UserId { get; private set; }
		public UInt32 AuthKey { get; private set; }

	}
}
