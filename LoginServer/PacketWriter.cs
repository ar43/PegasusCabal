using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
	internal static class PacketWriter
	{
		public static void WriteByte(Deque<byte> data, byte val)
		{
			data.AddToBack(val);
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

		public static void WriteHeader(Deque<byte> data, UInt16 val)
		{
			var a = val & 0xFF;
			var b = (val & 0xFF00) >> 8;
			data.AddToFront((byte)b);
			data.AddToFront((byte)a);
		}

		public static void WriteNull(Deque<byte> data, int len)
		{
			for(int i = 0; i < len; i++)
			{
				data.AddToBack((byte)0);
			}
		}

		public static void WriteArray(Deque<byte> data, byte[] input)
		{
			for(int i = 0; i < input.Length; i++)
			{
				data.AddToBack(input[i]);
			}
		}
	}
}
