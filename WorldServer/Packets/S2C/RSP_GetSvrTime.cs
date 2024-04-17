using Google.Protobuf.WellKnownTypes;
using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_GetSvrTime : PacketS2C
	{
		long _time;
		Int16 _timezoneOffset;
		public RSP_GetSvrTime(long time, Int16 timezoneOffset) : base((UInt16)Opcode.GETSVRTIME)
		{
			_time = time;
			_timezoneOffset = timezoneOffset;
		}

		public override void WritePayload()
		{

			PacketWriter.WriteUInt64(_data, (ulong)_time);
			PacketWriter.WriteInt16(_data, _timezoneOffset);
		}
	}
}
