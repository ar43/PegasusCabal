using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;
using Shared.Protos;
using System.Net;

namespace LoginServer.Packets.S2C
{
	internal class NFY_ServerState : PacketS2C
	{
		ServerStateReply _reply;
		public NFY_ServerState(ServerStateReply reply) : base((UInt16)Opcode.SERVERSTATE)
		{
			_reply = reply;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, (byte)_reply.ServerCount);
			for (int i = 0; i < _reply.ServerCount; i++)
			{
				var server = _reply.Servers[i];
				PacketWriter.WriteByte(data, (byte)server.ServerId);
				PacketWriter.WriteByte(data, (byte)server.ServerFlag);
				PacketWriter.WriteUInt32(data, 0); // LanguageMaybe
				PacketWriter.WriteByte(data, 0); //unk
				PacketWriter.WriteByte(data, 0); //unk
				PacketWriter.WriteByte(data, (byte)server.ChannelCount);
				for (int j = 0; j < server.ChannelCount; j++)
				{
					var chan = server.Channels[j];
					var ipArray = chan.Ip.ToCharArray();
					Array.Resize(ref ipArray, 64);
					PacketWriter.WriteByte(data, (byte)server.ServerId);
					PacketWriter.WriteByte(data, (byte)chan.ChannelId);
					PacketWriter.WriteUInt16(data, (UInt16)chan.UserCount);
					PacketWriter.WriteUInt16(data, 0); //UsersInWarLobby
					PacketWriter.WriteUInt16(data, 0); //u2
					PacketWriter.WriteUInt16(data, 0); //CapellasInWar
					PacketWriter.WriteUInt16(data, 0); //ProcInWar
					PacketWriter.WriteUInt32(data, 0); //u3
					PacketWriter.WriteUInt16(data, 0); //CapellasInWar2
					PacketWriter.WriteUInt16(data, 0); //ProcInWar2
					PacketWriter.WriteUInt16(data, 0); //u4
					PacketWriter.WriteByte(data, 0); // min lvl
					PacketWriter.WriteByte(data, 0); // max lvl
					PacketWriter.WriteByte(data, 0); // min rank
					PacketWriter.WriteByte(data, 0); // max rank
					PacketWriter.WriteUInt16(data, (UInt16)chan.MaximumUserCount);
					PacketWriter.WriteArray(data, System.Text.Encoding.UTF8.GetBytes(ipArray));
					PacketWriter.WriteUInt16(data, (UInt16)chan.Port);
					PacketWriter.WriteUInt64(data, chan.Type);
				}
			}
		}
	}
}
