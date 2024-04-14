using LibPegasus.Enums;
using LibPegasus.Packets;
using LoginServer.Enums;
using Shared.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets.S2C
{
	internal class NFY_ServerState : PacketS2C
	{
		ServerStateReply _reply;
		public NFY_ServerState(ServerStateReply reply) : base((UInt16)Opcode.SERVERSTATE)
		{
			_reply = reply;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteByte(_data, (byte)_reply.ServerCount);
			for(int i = 0; i < _reply.ServerCount; i++)
			{
				var server = _reply.Servers[i];
				PacketWriter.WriteByte(_data, (byte)server.ServerId);
				PacketWriter.WriteByte(_data, (byte)server.ServerFlag);
				PacketWriter.WriteUInt32(_data, 0); // LanguageMaybe
				PacketWriter.WriteByte(_data, (byte)server.ChannelCount);
				for(int j = 0; j < server.ChannelCount; j++)
				{
					var chan = server.Channels[j];
					var ip = BitConverter.ToUInt32(IPAddress.Parse(chan.Ip).GetAddressBytes(),0);
					PacketWriter.WriteByte(_data, (byte)chan.ChannelId);
					PacketWriter.WriteUInt16(_data, (UInt16)chan.UserCount);
					PacketWriter.WriteNull(_data, 21); //check ostara packet
					PacketWriter.WriteByte(_data, 0xFF); // maximum rank
					PacketWriter.WriteUInt16(_data, (UInt16)chan.MaximumUserCount);
					PacketWriter.WriteUInt32(_data, ip);
					PacketWriter.WriteUInt16(_data, (UInt16)chan.Port);
					PacketWriter.WriteUInt32(_data, chan.Type);
				}
			}
		}
	}
}
