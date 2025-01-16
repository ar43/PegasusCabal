namespace WorldServer.Logic.CharData.Styles
{
	internal class StyleEx
	{
		public Byte Sword { get; private set; } //probably BM3 suit
		public Byte AstralWeapon { get; private set; }
		public Byte Arrow { get; private set; } //perhaps mage weapon
		public Byte Aura { get; private set; }
		public Byte BM1 { get; private set; }
		public Byte BM2 { get; private set; }
		public Byte BM3 { get; private set; }
		public Byte ComboSkill { get; private set; }

		public void ToggleAstralWeapon(bool activate)
		{
			AstralWeapon = Convert.ToByte(activate);
		}

		public Byte Debug(Byte dbg)
		{
			return (Byte)dbg;
		}

		public Byte Serialize()
		{
			UInt32 result = 0;
			result |= Sword;
			result |= (UInt32)AstralWeapon << 1;
			result |= (UInt32)Arrow << 2;
			result |= (UInt32)Aura << 3;
			result |= (UInt32)BM1 << 4;
			result |= (UInt32)BM2 << 5;
			result |= (UInt32)BM3 << 6;
			result |= (UInt32)ComboSkill << 7;

			return (Byte)result;
		}
	}
}
