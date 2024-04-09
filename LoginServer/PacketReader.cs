using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
	internal static class PacketReader
	{
		public static byte ReadByte(Queue<byte> data)
		{
			if(data.Count <= 0)
			{
				throw new IndexOutOfRangeException("Data queue is empty");
			}
			else
			{
				return data.Dequeue();
			}
		}

		public static sbyte ReadInt8(Queue<byte> data)
		{
			try
			{
				return (sbyte)ReadByte(data);
			}
			catch
			{
				throw;
			}
		}

		public static UInt16 ReadUInt16(Queue<byte> data)
		{
			try
			{
				var a = ReadByte(data);
				var b = ReadByte(data);
				UInt16 result = (UInt16)(a | (b << 8));
				return result;
			}
			catch
			{
				throw;
			}
		}

		public static Int16 ReadInt16(Queue<byte> data)
		{
			try
			{
				return (Int16)ReadUInt16(data);
			}
			catch
			{
				throw;
			}
		}

		public static UInt32 ReadUInt32(Queue<byte> data)
		{
			try
			{
				var a = ReadByte(data);
				var b = ReadByte(data);
				var c = ReadByte(data);
				var d = ReadByte(data);
				return (UInt32)(a | (b << 8) | (c << 16) | (d << 24));
			}
			catch
			{
				throw;
			}
		}

		public static Int32 ReadInt32(Queue<byte> data)
		{
			try
			{
				return (Int32)ReadUInt32(data);
			}
			catch
			{
				throw;
			}
		}

		public static void ReadDiscard(Queue<byte> data, int count)
		{
			try
			{
				for(int i = 0; i < count; i++)
				{
					ReadByte(data);
				}
			}
			catch
			{
				throw;
			}
		}

		public static string ReadString(Queue<byte> data, int len) 
		{
			try
			{
				var byteList = new List<byte>();

				for(int i = 0; i < len; i++)
				{
					byte b = ReadByte(data);
					if(b != 0)
						byteList.Add(b);
				}

				return System.Text.Encoding.ASCII.GetString(byteList.ToArray());
			}
			catch
			{
				throw;
			}
		}

		public static byte[] ReadArray(Queue<byte> data)
		{
			if(data.Count == 0)
			{
				throw new IndexOutOfRangeException("Data queue is empty");
			}

			byte[] output = new byte[data.Count];

			int i = 0;
			while(data.Count > 0)
			{
				byte b = ReadByte(data);
				output[i] = (byte)b;
				i++;
			}

			return output;
		}
	}
}
