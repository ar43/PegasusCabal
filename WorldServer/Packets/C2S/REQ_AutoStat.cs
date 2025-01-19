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
	internal class REQ_AutoStat : PacketC2S<Client>
	{
		public REQ_AutoStat(Queue<byte> data) : base((UInt16)Opcode.CSC_AUTOSTAT, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			int str, dex, intelligence;

			try
			{
				str = PacketReader.ReadInt32(_data);
				dex = PacketReader.ReadInt32(_data);
				intelligence = PacketReader.ReadInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Interface.OnAutoStat(client, str, dex, intelligence));

			return true;
		}
	}
}
