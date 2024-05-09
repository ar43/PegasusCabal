using LibPegasus.Enums;
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
	internal class RSP_NewMyChartr : PacketS2C
	{
		private UInt32 _charId;
		private CharCreateResult _result;

		public RSP_NewMyChartr(UInt32 charId, CharCreateResult result) : base((UInt16)Opcode.CSC_NEWMYCHARTR)
		{
			_charId = charId;
			_result = result;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _charId);
			PacketWriter.WriteByte(data, (byte)_result);
		}
	}
}
