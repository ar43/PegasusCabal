using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	//TODO: this packet is different, depending on skill group
	internal class RSP_SkillToUser : PacketS2C
	{
		UInt16 _skillId;
		UInt16 _manaNew;
		UInt16 _state;
		UInt32 _u0;

		public RSP_SkillToUser(UInt16 skillId, UInt16 manaNew, UInt16 state, UInt32 u0) : base((UInt16)Opcode.CSC_SKILLTOUSER)
		{
			_skillId = skillId;
			_manaNew = manaNew;
			_state = state;
			_u0 = u0;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _skillId);
			PacketWriter.WriteUInt16(data, _manaNew);
			PacketWriter.WriteUInt16(data, _state);
			PacketWriter.WriteUInt32(data, _u0);
		}
	}
}
