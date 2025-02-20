﻿using LibPegasus.Packets;
using Nito.Collections;
using System.Net;
using System.Text;
using WorldServer.Enums;
using WorldServer.Logic.CharData;

namespace WorldServer.Packets.S2C
{
	internal class RSP_Initialized : PacketS2C
	{
		Character _character;
		UInt16 _userCount;
		IPAddress _chatServerIp;
		UInt16 _chatServerPort;
		UInt32 _worldId;

		public RSP_Initialized(Character character, ushort userCount, IPAddress chatServerIp, UInt16 chatServerPort, UInt32 worldId) : base((UInt16)Opcode.CSC_INITIALIZED)
		{
			_character = character;
			_userCount = userCount;
			_chatServerIp = chatServerIp;
			_chatServerPort = chatServerPort;
			_worldId = worldId;
		}

		public override void WritePayload(Deque<byte> data)
		{
			var cfg = ServerConfig.Get();
			UInt16 maxUserCount = 60; //add this to ServerConfig
			var ip = BitConverter.ToUInt32(IPAddress.Parse("127.0.0.1").GetAddressBytes(), 0); //fixme
			var chatIp = BitConverter.ToUInt32(_chatServerIp.GetAddressBytes(), 0);
			UInt32 channelType = 0; //fixme
			byte[] completedQuests = new byte[1023];
			byte[] completedDungeons = new byte[128];
			_character.QuestManager.CompletedQuests.RightShift(8); //hackerman
			_character.QuestManager.CompletedQuests.CopyTo(completedQuests, 0);
			_character.QuestManager.CompletedQuests.LeftShift(8);

			_character.QuestManager.CompletedDungeons.CopyTo(completedDungeons, 0);

			PacketWriter.WriteNull(data, 57); //unknown
			PacketWriter.WriteByte(data, 0); //unknown
			PacketWriter.WriteByte(data, 20); //unknown
			PacketWriter.WriteByte(data, (Byte)cfg.GeneralSettings.ChannelId);
			PacketWriter.WriteUInt16(data, _userCount);
			PacketWriter.WriteNull(data, 8 + 8 + 2); //unknown
			PacketWriter.WriteByte(data, 0); //minlvl?
			PacketWriter.WriteByte(data, 0); //maxlvl?
			PacketWriter.WriteByte(data, 0); //minrank?
			PacketWriter.WriteByte(data, 255); //maxrank?
			PacketWriter.WriteUInt16(data, maxUserCount);
			PacketWriter.WriteUInt32(data, ip);
			PacketWriter.WriteUInt16(data, (UInt16)cfg.ConnectionSettings.Port);
			PacketWriter.WriteUInt32(data, channelType);
			PacketWriter.WriteUInt16(data, _character.ObjectIndexData.ObjectId);
			PacketWriter.WriteByte(data, _character.ObjectIndexData.WorldIndex); //WorldIndex of ObjectIndexData??? todo Is this perhaps native instance support
			PacketWriter.WriteByte(data, (byte)_character.ObjectIndexData.ObjectType);

			PacketWriter.WriteUInt32(data, _worldId);
			PacketWriter.WriteUInt32(data, 0); //todo dungeon
			PacketWriter.WriteUInt16(data, (UInt16)_character.Location.Movement.X);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Location.Movement.Y);
			PacketWriter.WriteUInt64(data, _character.Stats.Exp);
			PacketWriter.WriteUInt64(data, _character.Inventory.Alz);
			PacketWriter.WriteUInt64(data, 0); //todo warxp
			PacketWriter.WriteUInt32(data, (UInt32)_character.Stats.Level);
			PacketWriter.WriteUInt32(data, 0); //unknown

			PacketWriter.WriteUInt32(data, (UInt32)_character.Stats.Str);
			PacketWriter.WriteUInt32(data, (UInt32)_character.Stats.Dex);
			PacketWriter.WriteUInt32(data, (UInt32)_character.Stats.Int);
			PacketWriter.WriteUInt32(data, (UInt32)_character.Stats.Pnt);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Stats.Rank); //magic and sword skill rank
			PacketWriter.WriteUInt16(data, 0); //unknown
			PacketWriter.WriteUInt32(data, 0); //unknown
			PacketWriter.WriteUInt16(data, (UInt16)_character.Status.MaxHp);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Status.Hp);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Status.MaxMp);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Status.Mp);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Status.MaxSp);
			PacketWriter.WriteUInt16(data, (UInt16)_character.Status.Sp);
			PacketWriter.WriteUInt16(data, 0); //dg points
			PacketWriter.WriteUInt16(data, 0); //dg points ??
			PacketWriter.WriteUInt32(data, 10800); //unknown
			PacketWriter.WriteUInt32(data, 0); //unknown
			PacketWriter.WriteUInt16(data, 0); //sword xp
			PacketWriter.WriteUInt16(data, 0); //sword point
			PacketWriter.WriteUInt16(data, 0); //magic xp
			PacketWriter.WriteUInt16(data, 0); //magic point
			PacketWriter.WriteUInt16(data, 0); //sword xp point
			PacketWriter.WriteUInt16(data, 0); //magic xp point
			PacketWriter.WriteUInt32(data, 0); //unknown
			PacketWriter.WriteUInt32(data, 0); //unknown
			PacketWriter.WriteUInt32(data, 0); //honor
			PacketWriter.WriteUInt64(data, 0); //death penalty xp
			PacketWriter.WriteUInt64(data, 0); //death penalty hp
			PacketWriter.WriteUInt64(data, 0); //death penalty mp
			PacketWriter.WriteUInt16(data, 0); //pk penalty

			PacketWriter.WriteUInt32(data, chatIp); //chat ip
			PacketWriter.WriteUInt16(data, _chatServerPort); //chat port

			PacketWriter.WriteUInt32(data, ip); //agent ip
			PacketWriter.WriteUInt16(data, 27096); //agent port

			PacketWriter.WriteByte(data, (Byte)_character.Nation); //nation
			PacketWriter.WriteUInt32(data, 0); //???
			PacketWriter.WriteUInt32(data, 7); //warps
			PacketWriter.WriteUInt32(data, 7); //maps
			PacketWriter.WriteUInt32(data, _character.Style.Serialize());
			PacketWriter.WriteUInt32(data, 0); //live style
			PacketWriter.WriteNull(data, 35); //??

			PacketWriter.WriteUInt16(data, _character.Equipment.Count());
			PacketWriter.WriteUInt16(data, (UInt16)_character.Inventory.Count()); //inv count
			PacketWriter.WriteUInt16(data, (UInt16)_character.Skills.Count()); //skill count
			PacketWriter.WriteUInt16(data, (UInt16)_character.QuickSlotBar.Count()); //quickslot count
			PacketWriter.WriteUInt16(data, 0); //mercenary count
			PacketWriter.WriteUInt16(data, 0); //??
			PacketWriter.WriteUInt16(data, 0); //??
			PacketWriter.WriteUInt16(data, 0); //ap
			PacketWriter.WriteUInt32(data, 0); //axp
			PacketWriter.WriteUInt16(data, 0); //??
			PacketWriter.WriteByte(data, 0); //bb count
			PacketWriter.WriteByte(data, (byte)_character.QuestManager.ActiveQuests.Count); //active quest count
			PacketWriter.WriteUInt16(data, 0); //Period Item Count

			PacketWriter.WriteArray(data, completedQuests); // quests completed - bitfield
			PacketWriter.WriteNull(data, 128); //quest dung flag
			PacketWriter.WriteArray(data, completedDungeons); // mission dungeons completed - bitfield

			PacketWriter.WriteByte(data, 1); //clvl0
			PacketWriter.WriteByte(data, 1); //clvl1
			PacketWriter.WriteByte(data, 1); //clvl2 
			PacketWriter.WriteByte(data, 1); //clvl3
			PacketWriter.WriteByte(data, 1); //clvl4
			PacketWriter.WriteUInt16(data, 0); //craft exp 0
			PacketWriter.WriteUInt16(data, 0); //craft exp 1
			PacketWriter.WriteUInt16(data, 0); //craft exp 2
			PacketWriter.WriteUInt16(data, 0); //craft exp 3
			PacketWriter.WriteUInt16(data, 0); //craft exp 4

			PacketWriter.WriteNull(data, 16); //craft flags

			PacketWriter.WriteUInt32(data, 0); //craft types
			PacketWriter.WriteUInt32(data, 0); //help window index

			PacketWriter.WriteNull(data, 163); //???
			PacketWriter.WriteNull(data, 12 * 4); //achievs
			PacketWriter.WriteUInt32(data, 0); //idk
			PacketWriter.WriteUInt32(data, 0); //quest count
			PacketWriter.WriteUInt32(data, 0); //quest flag count
			PacketWriter.WriteUInt32(data, 0); //unknown
			PacketWriter.WriteByte(data, (Byte)(_character.Name.Length + 1));
			PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(_character.Name), _character.Name.Length);

			PacketWriter.WriteArray(data, _character.Equipment.Serialize());
			PacketWriter.WriteArray(data, _character.Inventory.Serialize());
			PacketWriter.WriteArray(data, _character.Skills.Serialize());
			PacketWriter.WriteArray(data, _character.QuickSlotBar.Serialize());
			PacketWriter.WriteArray(data, _character.QuestManager.Serialize());

		}
	}
}
