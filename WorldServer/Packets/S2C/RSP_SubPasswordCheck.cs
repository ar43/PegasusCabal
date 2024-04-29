using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_SubPasswordCheck : PacketS2C
	{
		private UInt32 _result;
		private byte _failureCount;
		private SubPasswordType _subPasswordType;
		private SubPasswordLockType _subPasswordLockType;

		public RSP_SubPasswordCheck(UInt32 result, byte failureCount, SubPasswordType subPasswordType, SubPasswordLockType subPasswordLockType) : base((UInt16)Opcode.SUBPASSWORDCHECK)
		{
			_result = result;
			_failureCount = failureCount;
			_subPasswordType = subPasswordType;
			_subPasswordLockType = subPasswordLockType;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteUInt32(_data, _result);
			PacketWriter.WriteByte(_data, _failureCount);
			PacketWriter.WriteUInt32(_data, (UInt32)_subPasswordLockType);
			PacketWriter.WriteUInt32(_data, (UInt32)_subPasswordType);
		}
	}
}
