using LibPegasus.Enums;
using LoginServer.Enums;
using LoginServer.Opcodes;
using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets.S2C
{
	internal class RSP_AuthAccount : PacketS2C
	{
		LoginAccountReply _reply;

		public RSP_AuthAccount(LoginAccountReply reply) : base(Opcode.AUTHACCOUNT)
		{
			_reply = reply;
		}

		public override void WritePayload()
		{
			if((byte)_reply.Status == (byte)AuthResult.Normal)
			{
				PacketWriter.WriteByte(_data, (byte)_reply.Status);
				PacketWriter.WriteUInt32(_data, _reply.AccountId);
				PacketWriter.WriteInt16(_data, 0); //unknown
				PacketWriter.WriteByte(_data, (byte)_reply.ServerCount);
				PacketWriter.WriteNull(_data, 8); // unknown
				PacketWriter.WriteUInt32(_data, _reply.PremServId);
				PacketWriter.WriteUInt32(_data, _reply.PremServExpired);
				PacketWriter.WriteByte(_data, 0); //unknown
				PacketWriter.WriteByte(_data, Convert.ToByte(_reply.SubPassSet));
				PacketWriter.WriteNull(_data, 7); // unknown
				PacketWriter.WriteInt32(_data, (int)_reply.Language);
				PacketWriter.WriteArray(_data, Encoding.ASCII.GetBytes(_reply.AuthKey));
				PacketWriter.WriteByte(_data, 0); //null byte after string
				PacketWriter.WriteArray(_data, _reply.CharData.ToByteArray());
			}
			else
			{
				PacketWriter.WriteByte(_data, (byte)_reply.Status);
				PacketWriter.WriteNull(_data, 70);
			}

		}
	}
}
