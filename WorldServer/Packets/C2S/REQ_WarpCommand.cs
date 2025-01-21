using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_WarpCommand : PacketC2S<Client>
	{
		public REQ_WarpCommand(Queue<byte> data) : base((UInt16)Opcode.CSC_WARPCOMMAND, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte npcId;
			UInt16 slot;
			UInt32 extraParams;
			UInt32 target, u0, u1;

			try
			{
				npcId = PacketReader.ReadByte(_data);
				slot = PacketReader.ReadUInt16(_data);
				extraParams = PacketReader.ReadUInt32(_data);
				target = PacketReader.ReadUInt32(_data);
				u0 = PacketReader.ReadUInt32(_data);
				u1 = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Warping.OnWarpCommand(client, npcId, slot, extraParams, target));

			return true;
		}
	}
}
