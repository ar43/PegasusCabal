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
			BattleStyle = battleStyle; //3
			Rank = rank; //5
			Face = face; //5
			HairColor = hairColor; //4
			HairStyle = hairStyle; ///5
			Aura = aura; //4
			Gender = gender; //1
			ShowHelmet = showHelmet; //1
		}

		public Style(UInt32 serial)
		{
			BattleStyle = (Byte)(serial & 0b111);
			Rank = (Byte)((serial >> 3) & 0b11111);
			Face = (Byte)((serial >> 8) & 0b11111);
			HairColor = (Byte)((serial >> 13) & 0b1111);
			HairStyle = (Byte)((serial >> 17) & 0b11111);
			Aura = (Byte)((serial >> 22) & 0b1111);
			Gender = (Byte)((serial >> 26) & 0b1);
			ShowHelmet = (Byte)((serial >> 27) & 0b1);
		}

		public UInt32 Serialize()
		{
			UInt32 result = 0;
			result |= (UInt32)BattleStyle;
			result |= ((UInt32)Rank << 3);
			result |= ((UInt32)Face << 8);
			result |= (UInt32)HairColor << 13;
			result |= (UInt32)HairStyle << 17;
			result |= (UInt32)Aura << 22;
			result |= (UInt32)Gender << 26;
			result |= (UInt32)ShowHelmet << 27;

			return result;
		}

		public bool Verify()
		{
			//TODO
			return true;
		}
	}
}
