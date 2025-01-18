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
	internal class REQ_QuestDungeonMobsActive : PacketC2S<Client>
	{
		public REQ_QuestDungeonMobsActive(Queue<byte> data) : base((UInt16)Opcode.REQ_QUESTDUNGEONMOBSACTIVE, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte active;

			try
			{
				active = PacketReader.ReadByte(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Dungeoneering.OnDungeonMobsActiveRequest(client, active));

			return true;
		}
	}
}
