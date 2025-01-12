using Serilog;
using System.Net;
using System.Net.NetworkInformation;
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

		public static (int, int, int)[]? IntArrayToTupleArray3(int[]? arrayIn)
		{
			if (arrayIn == null)
				return null;
			if (arrayIn.Length % 3 != 0)
				throw new Exception("expected % 3");
			(int, int, int)[] newArray = new (int, int, int)[arrayIn.Length / 3];
			var test = new[] { (1, 2, 3) };
			for(int i = 0; i < arrayIn.Length; i +=3)
			{
				newArray[i / 3] = (arrayIn[i], arrayIn[i + 1], arrayIn[i + 2]);
			}
			return newArray;
		}

		public static int[]? StringToIntArrayComplex(string str)
		{
			if (str == "<null>")
				return null;
			return str.Split([',', ':']).Select(n => Convert.ToInt32(n)).ToArray();
		}

		public static List<string>? GetCmdArgs(string cmd)
		{
			var result = cmd.Split('"')
					 .Select((element, index) => index % 2 == 0  // If even index
										   ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
										   : new string[] { element })  // Keep the entire item
					 .SelectMany(element => element).ToList();
			return result;
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
