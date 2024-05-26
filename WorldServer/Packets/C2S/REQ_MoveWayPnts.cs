using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_MoveWayPnts : PacketC2S<Client>
	{
		public REQ_MoveWayPnts(Queue<byte> data) : base((UInt16)Opcode.REQ_MOVEWAYPNTS, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 fromX, fromY, toX, toY;

			try
			{
				fromX = PacketReader.ReadUInt16(_data);
				fromY = PacketReader.ReadUInt16(_data);
				toX = PacketReader.ReadUInt16(_data);
				toY = PacketReader.ReadUInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Movement.OnMoveWaypoint(client, fromX, fromY, toX, toY));

			return true;
		}
	}
}
