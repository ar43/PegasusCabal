using LibPegasus.Enums;
using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;
using Shared.Protos;
using System.Text;

namespace LoginServer.Packets.S2C
{
	internal class RSP_AuthAccount : PacketS2C
	{
		LoginAccountReply _reply;

		public RSP_AuthAccount(LoginAccountReply reply) : base((UInt16)Opcode.AUTHACCOUNT)
		{
			_reply = reply;
		}

		public override void WritePayload(Deque<byte> data)
		{
			if ((byte)_reply.Status == (byte)AuthResult.Normal)
			{
				PacketWriter.WriteByte(data, (byte)_reply.Status);
				PacketWriter.WriteUInt32(data, _reply.AccountId);
				PacketWriter.WriteInt16(data, 0); //unknown
				PacketWriter.WriteByte(data, (byte)_reply.ServerCount);
				PacketWriter.WriteNull(data, 8); // unknown
				PacketWriter.WriteUInt32(data, _reply.PremServId);
				PacketWriter.WriteUInt32(data, _reply.PremServExpired);
				PacketWriter.WriteByte(data, 0); //unknown
				PacketWriter.WriteByte(data, Convert.ToByte(_reply.SubPassSet));
				PacketWriter.WriteNull(data, 7); // unknown
				PacketWriter.WriteInt32(data, (int)_reply.Language);
				PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(_reply.AuthKey));
				PacketWriter.WriteByte(data, 0); //null byte after string
				PacketWriter.WriteArray(data, _reply.CharData.ToByteArray());
			}
			else
			{
				PacketWriter.WriteByte(data, (byte)_reply.Status);
				PacketWriter.WriteNull(data, 70);
			}

		}
	}
}
