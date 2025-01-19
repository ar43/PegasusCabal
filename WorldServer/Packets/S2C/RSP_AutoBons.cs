using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_AutoStat : PacketS2C
	{
		int _str, _dex, _int;

		public RSP_AutoStat(Int32 str, Int32 dex, Int32 @int) : base((UInt16)Opcode.CSC_AUTOSTAT)
		{
			_str = str;
			_dex = dex;
			_int = @int;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _str);
			PacketWriter.WriteInt32(data, _dex);
			PacketWriter.WriteInt32(data, _int);
		}
	}
}
