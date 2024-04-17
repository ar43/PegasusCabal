using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_GetMyChartr : PacketS2C
	{
		public RSP_GetMyChartr() : base((UInt16)Opcode.GETMYCHARTR)
		{
		}

		public override void WritePayload()
		{
			//TODO: actually implement it
			PacketWriter.WriteNull(_data, 22);
		}
	}
}
