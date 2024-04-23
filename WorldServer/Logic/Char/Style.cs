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

		public bool Verify()
		{
			//TODO
			return true;
		}
	}
}
