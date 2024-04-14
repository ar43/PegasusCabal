using LibPegasus.Enums;
using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Packets.S2C
{
	internal class RSP_CheckVersion : PacketS2C
	{
		private UInt32 _clientVersion;

		public RSP_CheckVersion(UInt32 clientVersion) : base((UInt16)Opcode.CHECKVERSION)
		{
			_clientVersion = clientVersion;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteUInt32(_data, _clientVersion);
			PacketWriter.WriteUInt32(_data, 0);
			PacketWriter.WriteUInt32(_data, 0);
			PacketWriter.WriteUInt32(_data, 0);

		}
	}
}
