using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_NpcShopSync : PacketS2C
	{
		Int32 _unknown;
		public NFY_NpcShopSync(Int32 unknown) : base((UInt16)Opcode.NFY_NPCSHOPSYNC)
		{
			_unknown = unknown;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, _unknown);
		}
	}
}
