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
				PacketWriter.WriteByte(data, (byte)server.ChannelCount);
				for (int j = 0; j < server.ChannelCount; j++)
				{
					var chan = server.Channels[j];
					var ip = BitConverter.ToUInt32(IPAddress.Parse(chan.Ip).GetAddressBytes(), 0);
					PacketWriter.WriteByte(data, (byte)chan.ChannelId);
					PacketWriter.WriteUInt16(data, (UInt16)chan.UserCount);
					PacketWriter.WriteNull(data, 21); //check ostara packet
					PacketWriter.WriteByte(data, 0xFF); // maximum rank
					PacketWriter.WriteUInt16(data, (UInt16)chan.MaximumUserCount);
					PacketWriter.WriteUInt32(data, ip);
					PacketWriter.WriteUInt16(data, (UInt16)chan.Port);
					PacketWriter.WriteUInt32(data, chan.Type);
				}
			}
		}
	}
}
