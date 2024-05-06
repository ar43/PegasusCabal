using LibPegasus.Packets;
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

		public override void WritePayload()
		{
			//TODO: actually implement it
			PacketWriter.WriteBool(_data, _reply.IsPinSet);
			PacketWriter.WriteNull(_data, 13);
			PacketWriter.WriteUInt32(_data, _reply.LastCharId);
			PacketWriter.WriteUInt32(_data, _reply.CharacterOrder);
			foreach(var character in _reply.Characters)
			{
				PacketWriter.WriteUInt32(_data, character.CharacterId);
				PacketWriter.WriteUInt64(_data, (UInt64)character.CreationDate);
				PacketWriter.WriteUInt32(_data, character.Style);
				PacketWriter.WriteUInt32(_data, character.Level);
				PacketWriter.WriteUInt32(_data, character.Rank);
				PacketWriter.WriteUInt64(_data, character.Alz);
				PacketWriter.WriteNull(_data, 1);
				PacketWriter.WriteByte(_data, (byte)character.WorldId);
				PacketWriter.WriteUInt16(_data, (UInt16)character.X);
				PacketWriter.WriteUInt16(_data, (UInt16)character.Y);
				foreach(var eqSlot in character.Equipment)
				{
					PacketWriter.WriteUInt32(_data, eqSlot);
				}
				PacketWriter.WriteNull(_data, 88);
				PacketWriter.WriteByte(_data, (Byte)(character.Name.Length+1));
				PacketWriter.WriteArray(_data, Encoding.ASCII.GetBytes(character.Name), character.Name.Length);
				PacketWriter.WriteNull(_data, 1);
			}
		}
	}
}
