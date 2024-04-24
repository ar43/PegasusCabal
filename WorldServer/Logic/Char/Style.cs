using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.Char
{
	internal class Style
	{
		public byte BattleStyle {  get; private set; }
		public byte Rank { get; private set; }
		public byte Face { get; private set; }
		public byte HairColor { get; private set; }
		public byte HairStyle { get; private set; }
		public byte Aura { get; private set; }
		public byte Gender { get; private set; }
		public byte ShowHelmet { get; private set; }

		public Style(Byte battleStyle, Byte rank, Byte face, Byte hairColor, Byte hairStyle, Byte aura, Byte gender, Byte showHelmet)
		{
			BattleStyle = battleStyle;
			Rank = rank;
			Face = face;
			HairColor = hairColor;
			HairStyle = hairStyle;
			Aura = aura;
			Gender = gender;
			ShowHelmet = showHelmet;
		}

		public UInt32 GetSerialized()
		{
			UInt32 result = 0;
			result |= (UInt32)BattleStyle;
			result |= ((UInt32)Rank << 3);
			result |= ((UInt32)Face << 5);
			result |= (UInt32)HairColor << 5;
			result |= (UInt32)HairColor << 4;
			result |= (UInt32)Aura << 5;
			result |= (UInt32)Gender << 4;
			result |= (UInt32)ShowHelmet << 1;

			return result;
		}

		public bool Verify()
		{
			//TODO
			return true;
		}
	}
}
