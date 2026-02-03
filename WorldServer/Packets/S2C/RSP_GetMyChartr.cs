using LibPegasus.Packets;
using Nito.Collections;
using Shared.Protos;
using System.Text;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_GetMyChartr : PacketS2C
	{
		GetMyCharactersReply _reply;
		public RSP_GetMyChartr(GetMyCharactersReply reply) : base((UInt16)Opcode.CSC_GETMYCHARTR)
		{
			_reply = reply;
		}

		public override void WritePayload(Deque<byte> data)
		{
			//TODO: actually implement it
			PacketWriter.WriteBoolLarge(data, _reply.IsPinSet);
			PacketWriter.WriteUInt64(data, 0); //BattlefieldEntryFee
			PacketWriter.WriteByte(data, 0); //unk
			PacketWriter.WriteByte(data, 0); //unk
			PacketWriter.WriteUInt32(data, _reply.LastCharId);
			PacketWriter.WriteUInt32(data, _reply.CharacterOrder);
			PacketWriter.WriteUInt32(data, 0); //slots opened
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
			PacketWriter.WriteUInt32(data, 0); //unk
			foreach (var character in _reply.Characters)
			{
				PacketWriter.WriteUInt32(data, character.CharacterId);
				PacketWriter.WriteUInt64(data, (UInt64)character.CreationDate);
				PacketWriter.WriteUInt32(data, character.Style);
				PacketWriter.WriteUInt32(data, character.Level);
				PacketWriter.WriteUInt16(data, 0); //Overlord level
				PacketWriter.WriteUInt32(data, 0); //w1
				PacketWriter.WriteUInt32(data, 0); //w1
				PacketWriter.WriteUInt32(data, 0); //w1
				PacketWriter.WriteUInt32(data, character.Rank); //SkillRank
				PacketWriter.WriteByte(data, 0); //Nation
				PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(character.Name), character.Name.Length);
				PacketWriter.WriteNull(data, 17 -  character.Name.Length);
				PacketWriter.WriteUInt64(data, 0); //Honor Point
				PacketWriter.WriteUInt64(data, character.Alz);
				PacketWriter.WriteByte(data, (byte)character.WorldId);
				PacketWriter.WriteUInt16(data, (UInt16)character.Y);
				PacketWriter.WriteUInt16(data, (UInt16)character.X);
				PacketWriter.WriteUInt16(data, (UInt16)character.EqCount);
				foreach (var eqSlot in character.Equipment.EquipmentData_)
				{
					var item = eqSlot.Value;
					PacketWriter.WriteUInt64(data, item.Kind);
					PacketWriter.WriteUInt64(data, item.Serial);
					PacketWriter.WriteUInt64(data, item.Option);
					PacketWriter.WriteUInt16(data, (UInt16)eqSlot.Key);
					PacketWriter.WriteUInt64(data, item.Duration);
				}
				for(int i = 0; i < (Int32)EquipmentIndex.NUM_EQUIPMENT - character.EqCount; i++)
				{
					PacketWriter.WriteNull(data, 30);
				}
				PacketWriter.WriteNull(data, 8 * 30);
				PacketWriter.WriteNull(data, 47 * 10);
			}
		}
	}
}
