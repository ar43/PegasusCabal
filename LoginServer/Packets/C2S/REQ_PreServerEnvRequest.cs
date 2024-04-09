using LoginServer.Logic.Delegates;
using LoginServer.Logic;
using LoginServer.Opcodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginServer.Enums;

namespace LoginServer.Packets.C2S
{
	internal class REQ_PreServerEnvRequest : PacketC2S
	{
		public REQ_PreServerEnvRequest(Queue<byte> data) : base(Opcode.PRESERVERENVREQUEST, data)
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
