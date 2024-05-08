namespace MasterServer.Chat
{
	internal enum ConnState
	{
		INITIAL,
		CONNECTED,
		VERSION_CHECKED,
		PRE_ENV,
		PUBLIC_KEY_REQUESTED,
		AUTH_ACCOUNT,
		VERIFYING,
		VERIFIED
	}
}
