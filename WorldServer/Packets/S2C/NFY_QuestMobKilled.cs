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
	internal class NFY_QuestMobKilled : PacketS2C
	{
		ushort _speciesId;
		int _skillId;

		public NFY_QuestMobKilled(UInt16 speciesId, Int32 skillId) : base((UInt16)Opcode.NFY_QUESTMOBKILLED)
		{
			_speciesId = speciesId;
			_skillId = skillId;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _speciesId);
			PacketWriter.WriteInt32(data, _skillId);
		}
	}
}
