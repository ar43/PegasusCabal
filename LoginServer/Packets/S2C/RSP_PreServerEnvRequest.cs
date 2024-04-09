using LoginServer.Enums;
using LoginServer.Opcodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets.S2C
{
	internal class RSP_PreServerEnvRequest : PacketS2C
	{
		public RSP_PreServerEnvRequest() : base(Opcode.PRESERVERENVREQUEST)
		{
		}

		public override void WritePayload()
		{
			PacketWriter.WriteNull(_data, 4113);
		}
	}
}
