using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_MoveTilePos : PacketC2S<Client>
	{
		public REQ_MoveTilePos(Queue<byte> data) : base((UInt16)Opcode.REQ_MOVETILEPOS, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 x, y;

			try
			{
				_ = PacketReader.ReadUInt32(_data);
				x = PacketReader.ReadUInt16(_data);
				y = PacketReader.ReadUInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Movement.OnTileChange(client, x, y));

			return true;
		}
	}
}
