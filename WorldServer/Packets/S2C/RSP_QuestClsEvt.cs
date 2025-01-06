using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_QuestClsEvt : PacketS2C
	{
		Byte _rewardType; //maybe this is result
		UInt16 _invSlot;
		UInt16 _u0, _u1;
		UInt32 _xpReward;

		public RSP_QuestClsEvt(Byte rewardType, UInt16 invSlot, UInt16 u0, UInt16 u1, UInt32 xpReward) : base((UInt16)Opcode.CSC_QUESTCLSEVT)
		{
			_rewardType = rewardType;
			_invSlot = invSlot;
			_u0 = u0;
			_u1 = u1;
			_xpReward = xpReward;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, _rewardType);
			PacketWriter.WriteUInt16(data, _invSlot);
			PacketWriter.WriteUInt16(data, _u0);
			PacketWriter.WriteUInt16(data, _u1);
			PacketWriter.WriteUInt32(data, _xpReward);
		}
	}
}
