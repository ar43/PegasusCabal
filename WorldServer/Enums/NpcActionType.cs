namespace WorldServer.Enums
{
	internal enum NpcActionType
	{
		QACT_GIVE = 0x00,
		QACT_TAKE,
		QACT_TALK,
		QACT_GMAP,              // MapCode
		QACT_GWAP,              // WarpCode

		QACT_QSTT,
		QACT_QEND,

		QACT_GALZ,              // ALZ
		QACT_TALZ,              // ALZ
		QACT_CTRG,              // Event Triger
		QACT_END0,
	}
}
