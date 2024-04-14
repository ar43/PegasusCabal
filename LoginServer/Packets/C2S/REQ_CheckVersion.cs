using LibPegasus.Logic.Delegates;
using LibPegasus.Logic;
using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibPegasus.Enums;

namespace LibPegasus.Packets.C2S
{
	internal class REQ_CheckVersion : PacketC2S<Client>
	{
		public REQ_CheckVersion(Queue<byte> data) : base((UInt16)Opcode.CHECKVERSION, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt32 clientVersion;

			try
			{
				clientVersion = PacketReader.ReadUInt32(_data);
				PacketReader.ReadDiscard(_data, 4 * 3);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnCheckVersion(x, clientVersion));

			return true;
		}
	}
}
