using LibPegasus.Packets;
using LibPegasus.Utils;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

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

			bool joinNoviceGuild;
			byte slot, nameLength;
			string name;
			try
			{
				style = PacketReader.ReadUInt32(_data);

				joinNoviceGuild = Convert.ToBoolean(PacketReader.ReadByte(_data));
				slot = PacketReader.ReadByte(_data);
				nameLength = PacketReader.ReadByte(_data);
				name = PacketReader.ReadString(_data, nameLength);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => CharSelect.OnCreate(x, style, joinNoviceGuild, slot, name));

			return true;
		}
	}
}
