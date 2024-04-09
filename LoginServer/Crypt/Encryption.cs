using LoginServer.Opcodes;
using LoginServer.Opcodes.C2S;
using Nito.Collections;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoginServer.Crypt
{
	internal class Encryption
	{
		bool _firstPacket = true;
		UInt32 _xorKeyTableBaseMultiple = 1;
		public UInt16 RecvXorKeyIdx {  get; private set; }
		UInt32 _recvXorKey = 0;

		public static readonly UInt32 RecvXorKeyNum = 0x4000;
		public static readonly UInt32 RecvXorKeyNumMask = 0x00003FFF;

		public static readonly UInt32 RecvXorSeed = 0x8F54C37B | 1;
		public static readonly UInt32 Recv2ndXorSeed = 0x34BC821A | 1;
		public static readonly UInt32 SendXorKey = 0x7AB38CF1;

		public static readonly UInt16 MagicKey = 0xB7E2;

		XorKeyTable _xorKeyTable;

		public Encryption(XorKeyTable xorKeyTable)
		{
			_xorKeyTable = xorKeyTable;
			RecvXorKeyIdx = (UInt16)RandomNumberGenerator.GetInt32(Convert.ToInt32(RecvXorKeyNum));
		}

		public UInt16 GetPacketSize(Span<byte> encryptedData)
		{
			if (_firstPacket)
			{
				return REQ_Connect2Serv.GetSize();
			}
			else
			{
				var span = new Span<byte>(encryptedData.ToArray(), 0, 4);
				UInt32 decryptedValue = BinaryPrimitives.ReadUInt32LittleEndian(span) ^ _recvXorKey;
				return (UInt16)(decryptedValue >> 16);
			}
		}

		public byte[] Encrypt(Deque<byte> byteQueue)
		{
			var packetLen = byteQueue.Count;
			byte[] outputBytes = byteQueue.ToArray();
			var span = new Span<byte>(outputBytes, 0, 4);

			var xorNum = BinaryPrimitives.ReadUInt32LittleEndian(span) ^ SendXorKey;
			BinaryPrimitives.WriteUInt32LittleEndian(span, xorNum);
			var xorKey = _xorKeyTable.KeyTable[xorNum & RecvXorKeyNumMask];

			var payloadLen = packetLen - (PacketS2C.HEADER_SIZE - 2);
			var j = 4;

			for (int i = 0; i < payloadLen / 4; i++)
			{
				span = new Span<byte>(outputBytes, j, 4);
				xorNum = BinaryPrimitives.ReadUInt32LittleEndian(span) ^ xorKey;
				BinaryPrimitives.WriteUInt32LittleEndian(span, xorNum);
				xorKey = _xorKeyTable.KeyTable[xorNum & RecvXorKeyNumMask];
				j += 4;
			}

			Int32 remainLen = packetLen % 4;
			if (remainLen > 0)
			{
				byte[] remainBytes = new byte[4];

				Array.Copy(outputBytes, j, remainBytes, 0, remainLen);

				UInt32 remainData = BinaryPrimitives.ReadUInt32LittleEndian(remainBytes);

				xorNum = remainData ^ xorKey;
				remainData = (xorNum); //???? this makes no sense at all

				BinaryPrimitives.WriteUInt32LittleEndian(remainBytes, remainData);

				Array.Copy(remainBytes, 0, outputBytes, j, remainLen);
			}


			return outputBytes;
		}

		public UInt16 Decrypt(byte[] data)
		{
			UInt16 packetLen = (UInt16)data.Length;
			var span = new Span<byte>(data, 0, 4);
			UInt32 header = BinaryPrimitives.ReadUInt32LittleEndian(span);

			if (_firstPacket)
			{
				_recvXorKey = (UInt32)(header ^ (MagicKey | (REQ_Connect2Serv.GetSize() << 16)));
			}

			var xorKey = _recvXorKey;
			var xorNum = header;

			header = xorNum ^ xorKey;
			xorKey = _xorKeyTable.KeyTable[(xorNum & RecvXorKeyNumMask) * _xorKeyTableBaseMultiple];
			BinaryPrimitives.WriteUInt32LittleEndian(span, header);

			span = new Span<byte>(data, 4, 4);
			BinaryPrimitives.WriteUInt32LittleEndian(span, 0);

			var payloadLen = packetLen - (PacketC2S.HEADER_SIZE - 2);
			var j = 8;

			for (int i = 0; i < payloadLen / 4; i++)
			{
				span = new Span<byte>(data, j, 4);
				xorNum = BinaryPrimitives.ReadUInt32LittleEndian(span);
				BinaryPrimitives.WriteUInt32LittleEndian(span, xorNum ^ xorKey);
				xorKey = _xorKeyTable.KeyTable[(xorNum & RecvXorKeyNumMask) * _xorKeyTableBaseMultiple];
				j += 4;
			}

			Int32 remainLen = packetLen % 4;
			if (remainLen > 0)
			{
				byte[] remainBytes = new byte[4];

				Array.Copy(data, j, remainBytes, 0, remainLen);

				UInt32 remainData = BinaryPrimitives.ReadUInt32LittleEndian(remainBytes);

				xorNum = remainData;
				remainData = (xorNum ^ xorKey);

				BinaryPrimitives.WriteUInt32LittleEndian(remainBytes, remainData);

				Array.Copy(remainBytes, 0, data, j, remainLen);
			}

			if (_firstPacket)
			{
				_firstPacket = false;
				_xorKeyTableBaseMultiple = 2;
			}

			_recvXorKey = _xorKeyTable.KeyTable[(RecvXorKeyIdx) * _xorKeyTableBaseMultiple];
			RecvXorKeyIdx++;

			if (RecvXorKeyIdx >= RecvXorKeyNum)
			{
				RecvXorKeyIdx = 0;
			}

			span = new Span<byte>(data, 8, 2);
			var opcode = BinaryPrimitives.ReadUInt16LittleEndian(span);

			return opcode;
		}
	}
}
