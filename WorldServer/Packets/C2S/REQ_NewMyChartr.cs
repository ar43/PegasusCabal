using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using WorldServer.Enums;
using LibPegasus.Utils;

namespace WorldServer.Packets.C2S
{
	internal class REQ_NewMyChartr : PacketC2S<Client>
	{
		public REQ_NewMyChartr(Queue<byte> data) : base((UInt16)Opcode.CSC_NEWMYCHARTR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt32 style;

			byte battleStyle, rank, face, hairColor, hairStyle, aura, gender, showHelmet;

			bool joinNoviceGuild;
			byte slot, nameLength;
			string name;
			try
			{
				style = PacketReader.ReadUInt32(_data);
				battleStyle = (byte)Utility.ReadBits(style, 0, 3);
				rank = (byte)Utility.ReadBits(style, 3, 5);
				face = (byte)Utility.ReadBits(style, 8, 5);
				hairColor = (byte)Utility.ReadBits(style, 13, 4);
				hairStyle = (byte)Utility.ReadBits(style, 17, 5);
				aura = (byte)Utility.ReadBits(style, 22, 4);
				gender = (byte)Utility.ReadBits(style, 26, 1);
				showHelmet = (byte)Utility.ReadBits(style, 27, 1);

				joinNoviceGuild = Convert.ToBoolean(PacketReader.ReadByte(_data));
				slot = PacketReader.ReadByte(_data);
				nameLength = PacketReader.ReadByte(_data);
				name = PacketReader.ReadString(_data, nameLength);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => CharSelect.OnCreate(x, battleStyle, rank, face, hairColor, hairStyle, aura, gender, showHelmet, joinNoviceGuild, slot, name));

			return true;
		}
	}
}
