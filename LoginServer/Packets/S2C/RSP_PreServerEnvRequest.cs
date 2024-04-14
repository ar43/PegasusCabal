using LibPegasus.Enums;
using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Packets.S2C
{
	internal class RSP_PreServerEnvRequest : PacketS2C
	{
		public RSP_PreServerEnvRequest() : base((UInt16)Opcode.PRESERVERENVREQUEST)
		{
		}

		public override void WritePayload()
		{
			PacketWriter.WriteNull(_data, 4113);
		}
	}
}
