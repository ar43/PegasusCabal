﻿using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_VerifyLinks : PacketC2S<Client>
	{
		public REQ_VerifyLinks(Queue<byte> data) : base((UInt16)Opcode.VERIFYLINKS, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt32 authKey;
			UInt16 userId;
			byte channelId;
			byte serverId;
			UInt32 clientMagicKey;

			try
			{
				authKey = PacketReader.ReadUInt32(_data);
				userId = PacketReader.ReadUInt16(_data);
				channelId = PacketReader.ReadByte(_data);
				serverId = PacketReader.ReadByte(_data);
				clientMagicKey = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnVerifyLinks(x, authKey, userId, channelId, serverId, clientMagicKey));

			return true;
		}
	}
}
