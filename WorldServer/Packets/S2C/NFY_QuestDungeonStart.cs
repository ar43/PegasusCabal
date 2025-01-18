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
	internal class NFY_QuestDungeonStart : PacketS2C
	{
		Int32 _timeLimit1, _timeLimit2, _u0;

		public NFY_QuestDungeonStart(Int32 timeLimit1, Int32 timeLimit2, Int32 u0) : base((UInt16)Opcode.NFY_QUESTDUNGEONSTART)
		{
			_timeLimit1 = timeLimit1;
			_timeLimit2 = timeLimit2;
			this._u0 = u0;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _timeLimit1);
			PacketWriter.WriteInt32(data, _timeLimit2);
			PacketWriter.WriteInt32(data, _u0);
		}
	}
}
