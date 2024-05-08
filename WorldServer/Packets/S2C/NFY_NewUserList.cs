using LibPegasus.Packets;
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

		public override void WritePayload()
		{
			PacketWriter.WriteByte(_data, (Byte)_characters.Count);
			PacketWriter.WriteByte(_data, (Byte)_newUserType);
			foreach(Character character in _characters)
			{
				PacketWriter.WriteUInt32(_data, (UInt32)character.Id);
				PacketWriter.WriteUInt16(_data, character.ObjectIndexData.UserId);
				PacketWriter.WriteByte(_data, character.ObjectIndexData.WorldIndex);
				PacketWriter.WriteByte(_data, (Byte)character.ObjectIndexData.ObjectType);
				PacketWriter.WriteUInt32(_data, character.Stats.Level);
				PacketWriter.WriteUInt32(_data, character.Stats.MoveSpeed);
				PacketWriter.WriteUInt16(_data, character.Location.X);
				PacketWriter.WriteUInt16(_data, character.Location.Y);
				PacketWriter.WriteUInt16(_data, character.Location.X);
				PacketWriter.WriteUInt16(_data, character.Location.Y);
				PacketWriter.WriteByte(_data, 0); //unknown
				PacketWriter.WriteInt32(_data, 0); //unknown
				PacketWriter.WriteInt16(_data, 0); //unknown
				PacketWriter.WriteUInt32(_data, character.Style.Serialize());
				PacketWriter.WriteByte(_data, (Byte)character.LiveStyle.Serialize()); //TODO: Why is this a byte? mistake?
				PacketWriter.WriteUInt16(_data, 0); //unknown
				PacketWriter.WriteUInt16(_data, character.Equipment.Count());
				PacketWriter.WriteInt16(_data, 0); //unknown
				PacketWriter.WriteNull(_data, 21); //unknown
				PacketWriter.WriteByte(_data, (Byte)(character.Name.Length + 1));
				PacketWriter.WriteArray(_data, Encoding.ASCII.GetBytes(character.Name), character.Name.Length);
				PacketWriter.WriteByte(_data, 0); //guild name len
				PacketWriter.WriteArray(_data, character.Equipment.SerializeEx());
			}
		}
	}
}
