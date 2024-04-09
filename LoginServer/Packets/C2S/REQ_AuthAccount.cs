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
	internal class REQ_AuthAccount : PacketC2S
	{
		public REQ_AuthAccount(Queue<byte> data) : base(Opcode.AUTHACCOUNT, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte[] rsaData;

			try
			{
				PacketReader.ReadDiscard(_data, 2);
				rsaData = PacketReader.ReadArray(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnAuthAccount(x, rsaData));

			return true;
		}
	}
}
