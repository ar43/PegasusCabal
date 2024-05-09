using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;

namespace WorldServer.Packets.C2S
{
	internal class REQ_MoveBegined : PacketC2S<Client>
	{
		public REQ_MoveBegined(Queue<byte> data) : base((UInt16)Opcode.REQ_MOVEBEGINED, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 fromX, fromY, toX, toY, pntX, pntY, worldId;

			try
			{
				fromX = PacketReader.ReadUInt16(_data);
				fromY = PacketReader.ReadUInt16(_data);
				toX = PacketReader.ReadUInt16(_data);
				toY = PacketReader.ReadUInt16(_data);
				pntX = PacketReader.ReadUInt16(_data);
				pntY = PacketReader.ReadUInt16(_data);
				worldId = PacketReader.ReadUInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Movement.OnMoveBegin(client, fromX, fromY, toX, toY, pntX, pntY, worldId));

			return true;
		}
	}
}
