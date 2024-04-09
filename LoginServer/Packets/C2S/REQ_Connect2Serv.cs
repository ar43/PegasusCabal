using LoginServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using LoginServer.Logic.Delegates;
using LoginServer.Logic;

namespace LoginServer.Opcodes.C2S
{
	internal class REQ_Connect2Serv : PacketC2S
	{
		public REQ_Connect2Serv(Queue<byte> data) : base(Opcode.CONNECT2SVR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			try
			{
				var reserved = PacketReader.ReadUInt32(_data);
			}
			catch(IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnServerConnection(x));

			return true;
		}

		public static UInt16 GetSize()
		{
			return 14;
		}
	}
}
