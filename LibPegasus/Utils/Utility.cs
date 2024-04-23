using Serilog;
using System.Text;

namespace LibPegasus.Utils
{
	public static class Utility
	{
		public static void PrintByteArray(byte[] bytes, int len, string Type)
		{
			var sb = new StringBuilder(Type + "(" + len.ToString() + "): \n");
			int i = 0;
			foreach (var b in bytes)
			{
				sb.Append(b.ToString("X2") + ", ");
				i++;
				if (i % 16 == 0)
					sb.Append("\n");
				if (i >= len)
					break;
			}
			//sb.Append("}");
			Log.Debug(sb.ToString());
		}

		public static UInt32 ReadBits(UInt32 input, int startBit, int length)
		{
			UInt32 mask = (UInt32)(((1 << length) - 1) << startBit);
			UInt32 result = input & mask;
			result = result >> startBit;
			return result;
		}

		public static void PrintCharArray(byte[] bytes, int len, string Type)
		{
			var sb = new StringBuilder(Type + "(" + len.ToString() + "): \n");
			int i = 0;
			foreach (var b in bytes)
			{
				string s;
				char c = Convert.ToChar(b);

				if (Char.IsControl(c))
					s = b.ToString("X2");
				else
					s = Convert.ToChar(b).ToString();

				sb.Append(s + ", ");
				i++;
				if (i % 16 == 0)
					sb.Append("\n");
				if (i >= len)
					break;
			}
			//sb.Append("}");
			Log.Debug(sb.ToString());
		}
	}
}
