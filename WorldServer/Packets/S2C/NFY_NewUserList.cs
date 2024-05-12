using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_NewUserList : PacketS2C
	{
		List<Character> _characters;
		NewUserType _newUserType;
		public NFY_NewUserList(List<Character> characters, NewUserType newUserType) : base((UInt16)Opcode.NFY_NEWUSERLIST)
		{
			_characters = characters;
			_newUserType = newUserType;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, (Byte)_characters.Count);
			PacketWriter.WriteByte(data, (Byte)_newUserType);
			foreach(Character character in _characters)
			{
				PacketWriter.WriteUInt32(data, (UInt32)character.Id);
				PacketWriter.WriteUInt16(data, character.ObjectIndexData.UserId);
				PacketWriter.WriteByte(data, character.ObjectIndexData.WorldIndex);
				PacketWriter.WriteByte(data, (Byte)character.ObjectIndexData.ObjectType);
				PacketWriter.WriteUInt32(data, character.Stats.Level);
				PacketWriter.WriteUInt32(data, (UInt32)(character.Location.Movement.MoveSpeed * 100)); //TODO: verify this cast...
				PacketWriter.WriteUInt16(data, (UInt16)character.Location.Movement.X);
				PacketWriter.WriteUInt16(data, (UInt16)character.Location.Movement.Y);

				//dest
				if(character.Location.Movement.IsMoving)
				{
					PacketWriter.WriteUInt16(data, (UInt16)character.Location.Movement.EndX); //TODO: perhaps PntX is meant here. need to verify
					PacketWriter.WriteUInt16(data, (UInt16)character.Location.Movement.EndY);
				}
				else
				{
					PacketWriter.WriteUInt16(data, (UInt16)character.Location.Movement.X);
					PacketWriter.WriteUInt16(data, (UInt16)character.Location.Movement.Y);
				}
				
				PacketWriter.WriteByte(data, 0); //unknown
				PacketWriter.WriteInt32(data, 0); //unknown
				PacketWriter.WriteInt16(data, 0); //unknown
				PacketWriter.WriteUInt32(data, character.Style.Serialize());
				PacketWriter.WriteByte(data, (Byte)character.LiveStyle.Serialize()); //TODO: Why is this a byte? mistake?
				PacketWriter.WriteUInt16(data, 0); //unknown
				PacketWriter.WriteUInt16(data, character.Equipment.Count());
				PacketWriter.WriteInt16(data, 0); //unknown
				PacketWriter.WriteNull(data, 21); //unknown
				PacketWriter.WriteByte(data, (Byte)(character.Name.Length + 1));
				PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(character.Name), character.Name.Length);
				PacketWriter.WriteByte(data, 0); //guild name len
				PacketWriter.WriteArray(data, character.Equipment.SerializeEx());
			}
		}
	}
}
