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
	internal class REQ_UseStatBons : PacketC2S<Client>
	{
		public REQ_UseStatBons(Queue<byte> data) : base((UInt16)Opcode.CSC_USESTATBONS, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			Byte stat;

			try
			{
				stat = PacketReader.ReadByte(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Interface.OnStatSpend(client, stat));

			return true;
		}
	}
}
