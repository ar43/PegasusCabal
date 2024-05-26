namespace MasterServer.Chat
{
	internal class ChatClientInfo
	{
		public static readonly int RSA_KEY_SIZE = 2048;

		public ChatClientInfo(UInt16 userId, UInt32 authKey)
		{
			UserId = userId;
			AuthKey = authKey;
			ConnState = ConnState.INITIAL;
			//RSA = RSA.Create(RSA_KEY_SIZE);
			//Username = "";
			AccountId = 0;
		}

		public UInt16 UserId { get; private set; }
		public UInt32 AuthKey { get; private set; }
		public ConnState ConnState;
		//public RSA RSA { get; private set; }

		public UInt32 AccountId;
	}
}
