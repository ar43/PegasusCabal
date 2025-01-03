using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.Styles;

namespace WorldServer.Packets.S2C
{
	//TODO: this packet is different, depending on skill group
	internal class NFY_SkillToUser : PacketS2C
	{
		UInt16 _skillId;
		UInt32 _charId;
		Style _style;
		LiveStyle _liveStyle;
		UInt16 _state;
		UInt32 _u0;

		public NFY_SkillToUser(UInt16 skillId, UInt32 charId, Style style, LiveStyle liveStyle, UInt16 state, UInt32 u0) : base((UInt16)Opcode.NFY_SKILLTOUSER)
		{
			_skillId = skillId;
			_charId = charId;
			_style = style;
			_liveStyle = liveStyle;
			_state = state;
			_u0 = u0;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _skillId);
			PacketWriter.WriteUInt32(data, _charId);
			PacketWriter.WriteUInt32(data, _style.Serialize());
			PacketWriter.WriteByte(data, _liveStyle.SerializeByte());
			PacketWriter.WriteByte(data, _style.StyleEx.Serialize());
			PacketWriter.WriteUInt16(data, _state);
			PacketWriter.WriteUInt32(data, _u0);
		}
	}
}
