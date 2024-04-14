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
	internal class REQ_PreServerEnvRequest : PacketC2S<Client>
	{
		public REQ_PreServerEnvRequest(Queue<byte> data) : base((UInt16)Opcode.PRESERVERENVREQUEST, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			string username;

			try
			{
				username = PacketReader.ReadString(_data, 33);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnPreServerEnvRequest(x, username));

			return true;
		}
	}
}
