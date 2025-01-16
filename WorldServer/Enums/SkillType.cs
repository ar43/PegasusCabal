namespace WorldServer.Enums
{
	internal enum SkillType
	{
		ST_NONESKIL,
		ST_SWRDSKIL,     // SwordSkill (0x01)
		ST_MAGCSKIL,     // MagicSkill (0x02)
		ST_SPECSKIL,                    // SpecialSkill
		ST_PASVSKIL,                    // PassiveSkill
		ST_COMBSKIL,                    // ComboSkill
		ST_CUTMSKIL,                    // CustomSkill

		// ----------------------------------------------------
		// Additional Type For SubType
		ST_ACTVSKIL,                    // ActiveSkill
		ST_BASCSKIL,                    // BasicSkill
										// ----------------------------------------------------

		// ----------------------------------------------------
		// For TrainType
		ST_TRNBYALZ,                    // TrainByAlz
		ST_TRNBYPNT,                    // TrainByPoint
		ST_TRNBYQST,                    // TrainByQuest
		ST_TRNBYALL,                    // TrainByAlz + TrainByQuest
										// ----------------------------------------------------
	}
}
