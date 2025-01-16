using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Packets.S2C.PacketSpecificData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_SkillToMobs : PacketS2C
	{
		UInt16 _skillId;
		int _characterId;
		UInt16 _x, _y;
		List<DamageToMobResult> _result;

		public NFY_SkillToMobs(List<DamageToMobResult> mobs, UInt16 skillId, Int32 characterId, UInt16 x, UInt16 y) : base((UInt16)Opcode.NFY_SKILLTOMOBS)
		{
			_skillId = skillId;
			_result = mobs;
			_characterId = characterId;
			_x = x;
			_y = y;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _skillId);
			PacketWriter.WriteByte(data, (byte)_result.Count);
			PacketWriter.WriteInt32(data, _characterId);
			PacketWriter.WriteUInt16(data, _x);
			PacketWriter.WriteUInt16(data, _y);

			foreach (var mobResult in _result)
			{
				PacketWriter.WriteUInt16(data, mobResult.ID.ObjectId);
				PacketWriter.WriteByte(data, mobResult.ID.WorldIndex);
				PacketWriter.WriteByte(data, (Byte)mobResult.ID.ObjectType);
				PacketWriter.WriteInt32(data, mobResult.Type); //mob type???
				PacketWriter.WriteByte(data, (Byte)mobResult.AttackResult);
				PacketWriter.WriteUInt32(data, mobResult.HPLeft);
				PacketWriter.WriteInt32(data, 0); //unknown TODO
				PacketWriter.WriteByte(data, 0); //unknown TODO
				PacketWriter.WriteInt32(data, 1); //unknown TODO
				PacketWriter.WriteInt32(data, 0); //unknown TODO
				PacketWriter.WriteByte(data, 0); //unknown TODO
			}

		}
	}
}
