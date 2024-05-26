using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_MoveEnded00 : PacketC2S<Client>
	{
		public REQ_MoveEnded00(Queue<byte> data) : base((UInt16)Opcode.REQ_MOVEENDED00, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 x, y;

			try
			{
				x = PacketReader.ReadUInt16(_data);
				y = PacketReader.ReadUInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Movement.OnMoveEnd(client, x, y));

			return true;
		}
	}
}
