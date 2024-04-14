using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
