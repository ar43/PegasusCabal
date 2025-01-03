using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.CharData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_ChangeStyle : PacketS2C
	{
		Character _character;
		public NFY_ChangeStyle(Character character) : base((UInt16)Opcode.NFY_CHANGESTYLE)
		{
			_character = character;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _character.Id);
			PacketWriter.WriteUInt32(data, _character.Style.Serialize());
			PacketWriter.WriteUInt32(data, _character.LiveStyle.Serialize());
			PacketWriter.WriteUInt32(data, _character.BuffManager.BuffFlag.Serialize());
			PacketWriter.WriteUInt16(data, _character.ActionFlag.Serialize());
		}
	}
}
