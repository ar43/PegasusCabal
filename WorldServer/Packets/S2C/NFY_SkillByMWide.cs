using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.SharedData;
using WorldServer.Packets.S2C.PacketSpecificData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_SkillByMWide : PacketS2C
	{
		ObjectIndexData _mobId;
		bool _isDefaultSkill;
		List<DamageFromMobResult> _results;

		public NFY_SkillByMWide(ObjectIndexData mobId, Boolean isDefaultSkill, List<DamageFromMobResult> results) : base((UInt16)Opcode.NFY_SKILLBYMWIDE)
		{
			_mobId = mobId;
			_isDefaultSkill = isDefaultSkill;
			_results = results;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _mobId.ObjectId);
			PacketWriter.WriteByte(data, _mobId.WorldIndex);
			PacketWriter.WriteByte(data, (Byte)_mobId.ObjectType);
			PacketWriter.WriteBool(data, _isDefaultSkill);
			PacketWriter.WriteUInt16(data, (UInt16)_results.Count);
			foreach (DamageFromMobResult result in _results)
			{
				PacketWriter.WriteInt32(data, result.CharId);
				PacketWriter.WriteBool(data, result.IsDead);
				PacketWriter.WriteByte(data, (Byte)result.AttackResult);
				PacketWriter.WriteUInt16(data, result.Damage);
				PacketWriter.WriteInt32(data, result.RemainingHp);
				PacketWriter.WriteNull(data, 2 + 4 + 4 + 4 + 1);
			}
		}
	}
}
