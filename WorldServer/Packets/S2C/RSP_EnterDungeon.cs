using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_EnterDungeon : PacketS2C
	{
		int u0;
		int DungeonId;
		int WarpType;
		int NpcId;
		int u2;
		int u3;
		int Map;

		public RSP_EnterDungeon(Int32 u0, Int32 dungeonId, Int32 warpType, Int32 npcId, Int32 u2, Int32 u3, Int32 map) : base((UInt16)Opcode.CSC_ENTERDUNGEON)
		{
			this.u0 = u0;
			DungeonId = dungeonId;
			WarpType = warpType;
			NpcId = npcId;
			this.u2 = u2;
			this.u3 = u3;
			Map = map;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteInt32(data, u0);
			PacketWriter.WriteInt32(data, DungeonId);
			PacketWriter.WriteInt32(data, WarpType);
			PacketWriter.WriteInt32(data, NpcId);
			PacketWriter.WriteInt32(data, u2);
			PacketWriter.WriteInt32(data, u3);
			PacketWriter.WriteInt32(data, Map);
		}
	}
}
