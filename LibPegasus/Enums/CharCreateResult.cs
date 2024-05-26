namespace LibPegasus.Enums
{
	public enum CharCreateResult
	{
		DBERROR = 0x01,
		DATABRK = 0x02,
		NAMEDUP = 0x03,
		BADWORD = 0x04,
		SUCCESS = 0xA1,
		FAIL_DELETE_24HOUR = 0xB1,
		FAIL_DELETE_GUILDMASTER = 0xB2,
		FAIL_JOIN_BEGINNER_GUILD = 0xB3,
	}
}
