using Nito.Collections;
using System.Text;

namespace LibPegasus.Packets
{
	public static class PacketWriter
	{
		public static void WriteByte(Deque<byte> data, byte val)
		{
			data.AddToBack(val);
		}

		public static void WriteBool(Deque<byte> data, bool val)
		{
			data.AddToBack(Convert.ToByte(val));
		}

		public static void WriteInt8(Deque<byte> data, sbyte val)
		{
			data.AddToBack((byte)val);
		}

		public static void WriteUInt16(Deque<byte> data, UInt16 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			data.AddToBack((byte)a);
			data.AddToBack((byte)b);
		}

		public static void WriteInt16(Deque<byte> data, Int16 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			data.AddToBack((byte)a);
			data.AddToBack((byte)b);
		}

		public static void WriteUInt32(Deque<byte> data, UInt32 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			var c = (val & 0xFF0000) >> 16;
			var d = (val & 0xFF000000) >> 24;
			data.AddToBack((byte)a);
			data.AddToBack((byte)b);
			data.AddToBack((byte)c);
			data.AddToBack((byte)d);
		}

		public static void WriteInt32(Deque<byte> data, Int32 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			var c = (val & 0xFF0000) >> 16;
			var d = (val & 0xFF000000) >> 24;
			data.AddToBack((byte)a);
			data.AddToBack((byte)b);
			data.AddToBack((byte)c);
			data.AddToBack((byte)d);
		}

		public static void WriteUInt64(Deque<byte> data, UInt64 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			var c = (val & 0xFF0000) >> 16;
			var d = (val & 0xFF000000) >> 24;
			var e = (val & 0xFF00000000) >> 32;
			var f = (val & 0xFF0000000000) >> 40;
			var g = (val & 0xFF000000000000) >> 48;
			var h = (val & 0xFF00000000000000) >> 56;
			data.AddToBack((byte)a);
			data.AddToBack((byte)b);
			data.AddToBack((byte)c);
			data.AddToBack((byte)d);
			data.AddToBack((byte)e);
			data.AddToBack((byte)f);
			data.AddToBack((byte)g);
			data.AddToBack((byte)h);
		}

		public static void WriteHeader(Deque<byte> data, UInt16 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			data.AddToFront((byte)b);
			data.AddToFront((byte)a);
		}

		public static void WriteNull(Deque<byte> data, int len)
		{
			for (int i = 0; i < len; i++)
			{
				data.AddToBack((byte)0);
			}
		}

		public static void WriteArray(Deque<byte> data, byte[] input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				data.AddToBack(input[i]);
			}
		}

		public static void WriteArray(Deque<byte> data, byte[] input, int len)
		{
			for (int i = 0; i < len; i++)
			{
				data.AddToBack(input[i]);
			}
		}

		public static void WriteString(Deque<Byte> data, String message)
		{
			WriteArray(data, Encoding.ASCII.GetBytes(message));
		}
	}
}
