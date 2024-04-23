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
	internal class RSP_NewMyChartr : PacketS2C
	{
		private UInt32 _charId;
		private CharCreateResult _result;

		public RSP_NewMyChartr(UInt32 charId, CharCreateResult result) : base((UInt16)Opcode.NEWMYCHARTR)
		{
			_charId = charId;
			_result = result;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteUInt32(_data, _charId);
			PacketWriter.WriteByte(_data, (byte)_result);
		}
	}
}
