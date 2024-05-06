using LibPegasus.Enums;
using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_SubPasswordCheckRequest : PacketS2C
	{
		private bool _pinRequired;

		public RSP_SubPasswordCheckRequest(bool pinRequired) : base((UInt16)Opcode.CSC_SUBPASSWORDCHECKREQUEST)
		{
			_pinRequired = pinRequired;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteBool(_data, _pinRequired);
			PacketWriter.WriteNull(_data, 3);
		}
	}
}
