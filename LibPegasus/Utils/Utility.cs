using Serilog;
using System.Net;
using System.Security.Cryptography;
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

		public static UInt32 ReverseBytes(UInt32 value)
		{
			return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
				(value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
		}

		public static int[]? StringToIntArray(string str)
		{
			if (str == "<null>")
				return null;
			return str.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
		}

		public static UInt32 ReadBits(UInt32 input, int startBit, int length)
		{
			UInt32 mask = (UInt32)(((1 << length) - 1) << startBit);
			UInt32 result = input & mask;
			result = result >> startBit;
			return result;
		}

		public static async Task<IPAddress?> GetExternalIpAddress()
		{
			var externalIpString = (await new HttpClient().GetStringAsync("http://icanhazip.com"))
				.Replace("\\r\\n", "").Replace("\\n", "").Trim();
			if (!IPAddress.TryParse(externalIpString, out var ipAddress)) return null;
			return ipAddress;
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
