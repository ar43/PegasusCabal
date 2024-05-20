using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using WorldServer.Enums;

namespace WorldServer.Packets.C2S
{
	internal class REQ_Uninitalze : PacketC2S<Client>
	{
		public REQ_Uninitalze(Queue<byte> data) : base((UInt16)Opcode.CSC_UNINITIALZE, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 index;
			byte mapId, option;

			try
			{
				index = PacketReader.ReadUInt16(_data); //not sure
				mapId = PacketReader.ReadByte(_data); //initial map, what to do with this??
				option = PacketReader.ReadByte(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => IngameConnection.OnUninitialize(client, index, mapId, option));

			return true;
		}
	}
}
