﻿using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_VerifyLinks : PacketS2C
	{
		byte _channelId, _serverId;
		bool _isVerified;
		public RSP_VerifyLinks(byte channelId, byte serverId, bool isVerified) : base((UInt16)Opcode.CSC_VERIFYLINKS)
		{
			_channelId = channelId;
			_serverId = serverId;
			_isVerified = isVerified;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _channelId);
			PacketWriter.WriteByte(data, _serverId);
			PacketWriter.WriteByte(data, Convert.ToByte(_isVerified));
		}
	}
}
