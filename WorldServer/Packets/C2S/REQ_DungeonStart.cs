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
	internal class REQ_DungeonStart : PacketC2S<Client>
	{
		public REQ_DungeonStart(Queue<byte> data) : base((UInt16)Opcode.CSC_DUNGEONSTART, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			actions.Enqueue((client) => Dungeoneering.OnDungeonStart(client));

			return true;
		}
	}
}
