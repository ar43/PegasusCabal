using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_SkillToMobs : PacketS2C
	{
		UInt16 _skillId;
		Byte _u0;
		int _characterId;
		UInt16 _x, _y;
		ObjectIndexData _objectIndexData;
		int _u1;
		byte _mpMaybe;
		uint _hp;
		int _u3;
		byte _u15;
		int _u4;
		int _u16;
		byte _u17;

		public NFY_SkillToMobs(UInt16 skillId, Byte u0, Int32 characterId, UInt16 x, UInt16 y, ObjectIndexData objectIndexData, Int32 u1, Byte mpMaybe, UInt32 hp, Int32 u3, Byte u15, Int32 u4, Int32 u16, Byte u17) : base((UInt16)Opcode.NFY_SKILLTOMOBS)
		{
			_skillId = skillId;
			_u0 = u0;
			_characterId = characterId;
			_x = x;
			_y = y;
			_objectIndexData = objectIndexData;
			_u1 = u1;
			_mpMaybe = mpMaybe;
			_hp = hp;
			_u3 = u3;
			_u15 = u15;
			_u4 = u4;
			_u16 = u16;
			_u17 = u17;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _skillId);
			PacketWriter.WriteByte(data, _u0);
			PacketWriter.WriteInt32(data, _characterId);
			PacketWriter.WriteUInt16(data, _x);
			PacketWriter.WriteUInt16(data, _y);
			PacketWriter.WriteUInt16(data, _objectIndexData.ObjectId);
			PacketWriter.WriteByte(data, _objectIndexData.WorldIndex);
			PacketWriter.WriteByte(data, (Byte)_objectIndexData.ObjectType);
			PacketWriter.WriteInt32(data, _u1);
			PacketWriter.WriteByte(data, _mpMaybe);
			PacketWriter.WriteUInt32(data, _hp);
			PacketWriter.WriteInt32(data, _u3);
			PacketWriter.WriteByte(data, _u15);
			PacketWriter.WriteInt32(data, _u4);
			PacketWriter.WriteInt32(data, _u16);
			PacketWriter.WriteByte(data, _u17);
		}
	}
}
