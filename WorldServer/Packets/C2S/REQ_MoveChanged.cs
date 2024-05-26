using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_MoveChanged : PacketC2S<Client>
	{
		public REQ_MoveChanged(Queue<byte> data) : base((UInt16)Opcode.REQ_MOVECHANGED, data)
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

			actions.Enqueue((client) => Movement.OnMoveChanged(client, fromX, fromY, toX, toY, pntX, pntY, worldId));

			return true;
		}
	}
}
