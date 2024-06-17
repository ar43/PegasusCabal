using LibPegasus.Packets;
using Nito.Collections;
using System.Diagnostics;
using WorldServer.Enums;
using WorldServer.Packets.S2C.PacketSpecificData;

namespace WorldServer.Packets.S2C
{
	internal class RSP_SkillToMobs : PacketS2C
	{
		List<DamageToMobResult> _mobs;
		private ushort _skillId;
		private ushort _hp;
		private ushort _mp;
		private ushort _sp;
		private ulong _newXp;
		private uint _skillXp;
		private ushort _axp;
		private ushort u1;
		private ushort _mp2;

		public RSP_SkillToMobs(List<DamageToMobResult> mobs, UInt16 skillId, UInt16 hp, UInt16 mp, UInt16 sp, UInt64 newXP, UInt32 skillXP, UInt16 axp, UInt16 u1, UInt16 mp2) : base((UInt16)Opcode.CSC_SKILLTOMOBS)
		{
			Debug.Assert(mobs.Count > 0);
			_mobs = mobs;
			_skillId = skillId;
			_hp = hp;
			_mp = mp;
			_sp = sp;
			_newXp = newXP;
			_skillXp = skillXP;
			_axp = axp;
			this.u1 = u1;
			_mp2 = mp2;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _skillId);
			PacketWriter.WriteUInt16(data, _hp);
			PacketWriter.WriteUInt16(data, _mp);
			PacketWriter.WriteUInt16(data, _sp);
			PacketWriter.WriteUInt64(data, _newXp);
			PacketWriter.WriteUInt32(data, _skillXp);
			PacketWriter.WriteUInt16(data, _axp);
			PacketWriter.WriteUInt16(data, u1);//?? TODO maybe also axp
			PacketWriter.WriteUInt16(data, _mp2); //??
			PacketWriter.WriteNull(data, 24); //??
			PacketWriter.WriteByte(data, 255); //??
			PacketWriter.WriteByte(data, 255); //??
			PacketWriter.WriteByte(data, 255); //??
			PacketWriter.WriteByte(data, 255); //??
			PacketWriter.WriteByte(data, (Byte)_mobs.Count);
			for(int i = 0; i < (byte)_mobs.Count; i++)
			{
				PacketWriter.WriteUInt16(data, _mobs[i].ID.ObjectId);
				PacketWriter.WriteByte(data, _mobs[i].ID.WorldIndex);
				PacketWriter.WriteByte(data, (Byte)_mobs[i].ID.ObjectType);
				PacketWriter.WriteByte(data, _mobs[i].Type);
				PacketWriter.WriteByte(data, (Byte)_mobs[i].AttackResult);
				PacketWriter.WriteUInt16(data, _mobs[i].DamageReceived);
				PacketWriter.WriteUInt32(data, _mobs[i].HPLeft);
				PacketWriter.WriteUInt32(data, _mobs[i].u8);
				PacketWriter.WriteUInt32(data, _mobs[i].u9);
				PacketWriter.WriteByte(data, _mobs[i].u11);
				PacketWriter.WriteByte(data, _mobs[i].HasBFX);
			}
		}
	}
}
