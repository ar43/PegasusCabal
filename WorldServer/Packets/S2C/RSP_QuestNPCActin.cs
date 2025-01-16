using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_QuestNpcActin : PacketS2C
	{
		UInt16 _questId;
		UInt16 _flagId;
		UInt32 _setId;

		public RSP_QuestNpcActin(UInt16 questId, UInt16 flagId, UInt32 setId) : base((UInt16)Opcode.CSC_QUESTNPCACTIN)
		{
			_questId = questId;
			_flagId = flagId;
			_setId = setId;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, _questId);
			PacketWriter.WriteUInt16(data, _flagId);
			PacketWriter.WriteUInt32(data, _setId);
		}
	}
}
