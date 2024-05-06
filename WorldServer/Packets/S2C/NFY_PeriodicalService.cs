using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_PeriodicalService : PacketS2C
	{
		UInt32 _timeInSeconds;

		public NFY_PeriodicalService(UInt32 timeInSeconds) : base((UInt16)Opcode.NFY_PERIODICALSERVICE)
		{
			_timeInSeconds = timeInSeconds;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteUInt32(_data, 0); //unk
			PacketWriter.WriteUInt32(_data, 0); //unk
			PacketWriter.WriteUInt32(_data, _timeInSeconds);
			PacketWriter.WriteUInt32(_data, 0); //unk
		}
	}
}
