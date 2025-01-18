using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using System.Diagnostics;

namespace WorldServer.Packets.C2S
{
	internal class REQ_EnterDungeon : PacketC2S<Client>
	{
		public REQ_EnterDungeon(Queue<byte> data) : base((UInt16)Opcode.CSC_ENTERDUNGEON, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			int dungeonId, warpType, npcId, u2, u3, mapId;

			try
			{
				dungeonId = PacketReader.ReadInt32(_data);
				warpType = PacketReader.ReadInt32(_data);
				npcId = PacketReader.ReadInt32(_data);
				u2 = PacketReader.ReadInt32(_data);
				u3 = PacketReader.ReadInt32(_data);
				mapId = PacketReader.ReadInt32(_data);
				Debug.Assert(warpType == 1 || warpType == 2);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Warping.OnEnterDungeon(client, dungeonId, warpType, npcId, u2, u3, mapId));

			return true;
		}
	}
}
