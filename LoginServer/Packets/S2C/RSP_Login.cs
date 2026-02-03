using LibPegasus.Enums;
using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;
using Shared.Protos;
using System.Text;

namespace LoginServer.Packets.S2C
{
	internal class RSP_Login : PacketS2C
	{
		LoginAccountReply _reply;
		bool _subMessage;

		public RSP_Login(LoginAccountReply reply, bool subMessage) : base((UInt16)Opcode.LOGIN)
		{
			_reply = reply;
			_subMessage = subMessage;
		}

		public override void WritePayload(Deque<byte> data)
		{
			bool keepAlive = (byte)_reply.Status == (byte)AuthResult.Normal;
			UInt32 subMessageType = 0;
			UInt32 result = _reply.Status;
			if (keepAlive && _subMessage)
				subMessageType = 17;

			PacketWriter.WriteBool(data, keepAlive);
			PacketWriter.WriteUInt32(data, 0);
			PacketWriter.WriteUInt32(data, 0xFFFFFFFF);
			PacketWriter.WriteUInt32(data, 0);
			PacketWriter.WriteUInt32(data, subMessageType);
			PacketWriter.WriteUInt32(data, result);

			if (subMessageType == 17)
			{
				PacketWriter.WriteByte(data, 1); //??
				PacketWriter.WriteUInt32(data, _reply.AccountId);
				PacketWriter.WriteByte(data, 48);
				PacketWriter.WriteUInt32(data, 0);
				PacketWriter.WriteByte(data, 0);
				PacketWriter.WriteUInt32(data, 16777216);
				PacketWriter.WriteUInt32(data, 1392508928);
				PacketWriter.WriteUInt64(data, 7169838);
				PacketWriter.WriteUInt32(data, 1879048192);
				PacketWriter.WriteUInt64(data, 0); //PremiumServExpired
				PacketWriter.WriteUInt32(data, 1); //HasChars?
				PacketWriter.WriteNull(data, 3); //padding
				PacketWriter.WriteUInt32(data, 1); //??
				PacketWriter.WriteUInt32(data, 0); //??
				PacketWriter.WriteInt32(data, (int)_reply.Language);
				PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(_reply.AuthKey));
				PacketWriter.WriteByte(data, 0); //??
				PacketWriter.WriteByte(data, 1); //svr count

				//char count per server
				//seems to be bugged on ep33
				PacketWriter.WriteByte(data, 1);
				PacketWriter.WriteByte(data, 250);
				PacketWriter.WriteNull(data, 254);


				/*
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
				*/
			}

		}
	}
}
