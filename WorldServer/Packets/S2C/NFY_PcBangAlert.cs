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
	internal class NFY_PcBangAlert : PacketS2C
	{
		UInt32 _remainingTime, _remainingPoints;

		public NFY_PcBangAlert(UInt32 remainingTime, UInt32 remainingPoints) : base((UInt16)Opcode.NFY_PCBANGALERT)
		{
			_remainingTime = remainingTime;
			_remainingPoints = remainingPoints;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _remainingTime);
			PacketWriter.WriteUInt32(data, _remainingPoints);
		}
	}
}
