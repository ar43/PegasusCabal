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
			PacketWriter.WriteBool(data, _reply.IsPinSet);
			PacketWriter.WriteNull(data, 13);
			PacketWriter.WriteUInt32(data, _reply.LastCharId);
			PacketWriter.WriteUInt32(data, _reply.CharacterOrder);
			foreach(var character in _reply.Characters)
			{
				PacketWriter.WriteUInt32(data, character.CharacterId);
				PacketWriter.WriteUInt64(data, (UInt64)character.CreationDate);
				PacketWriter.WriteUInt32(data, character.Style);
				PacketWriter.WriteUInt32(data, character.Level);
				PacketWriter.WriteUInt32(data, character.Rank);
				PacketWriter.WriteUInt64(data, character.Alz);
				PacketWriter.WriteNull(data, 1);
				PacketWriter.WriteByte(data, (byte)character.WorldId);
				PacketWriter.WriteUInt16(data, (UInt16)character.X);
				PacketWriter.WriteUInt16(data, (UInt16)character.Y);
				foreach(var eqSlot in character.Equipment)
				{
					PacketWriter.WriteUInt32(data, eqSlot);
				}
				PacketWriter.WriteNull(data, 88);
				PacketWriter.WriteByte(data, (Byte)(character.Name.Length+1));
				PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(character.Name), character.Name.Length);
				PacketWriter.WriteNull(data, 1);
			}
		}
	}
}
