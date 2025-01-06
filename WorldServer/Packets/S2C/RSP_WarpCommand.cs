using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.CharData;

namespace WorldServer.Packets.S2C
{
	internal class RSP_WarpCommand : PacketS2C
	{
		Character _character;
		UInt32 _warpType;
		UInt32 _worldId;
		UInt32 _dungeonId;

		public RSP_WarpCommand(Character character, UInt32 warpType, UInt32 worldId, UInt32 dungeonId) : base((UInt16)Opcode.CSC_WARPCOMMAND)
		{
			_character = character;
			_warpType = warpType;
			_worldId = worldId;
			_dungeonId = dungeonId;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, (UInt16)_character.Location.Movement.X);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Location.Movement.Y);
			PacketWriter.WriteUInt64(data, _character.Stats.Exp);
			//PacketWriter.WriteUInt32(data, _character.Stats.Axp); NEED TO RESEARCH
			PacketWriter.WriteUInt64(data, _character.Inventory.Alz);
			PacketWriter.WriteUInt16(data, _character.ObjectIndexData.ObjectId);
			PacketWriter.WriteByte(data, _character.ObjectIndexData.WorldIndex);
			PacketWriter.WriteByte(data, (Byte)_character.ObjectIndexData.ObjectType);
			PacketWriter.WriteUInt32(data, _warpType);
			PacketWriter.WriteByte(data, 0); //unk
			PacketWriter.WriteUInt32(data, _worldId);
			PacketWriter.WriteUInt32(data, _dungeonId);
			PacketWriter.WriteUInt32(data, 0); //unk
		}
	}
}
