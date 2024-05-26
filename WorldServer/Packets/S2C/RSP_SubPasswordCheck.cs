using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_SubPasswordCheck : PacketS2C
	{
		private UInt32 _result;
		private byte _failureCount;
		private SubPasswordType _subPasswordType;
		private SubPasswordLockType _subPasswordLockType;

		public RSP_SubPasswordCheck(UInt32 result, byte failureCount, SubPasswordType subPasswordType, SubPasswordLockType subPasswordLockType) : base((UInt16)Opcode.CSC_SUBPASSWORDCHECK)
		{
			_result = result;
			_failureCount = failureCount;
			_subPasswordType = subPasswordType;
			_subPasswordLockType = subPasswordLockType;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _result);
			PacketWriter.WriteByte(data, _failureCount);
			PacketWriter.WriteUInt32(data, (UInt32)_subPasswordLockType);
			PacketWriter.WriteUInt32(data, (UInt32)_subPasswordType);
		}
	}
}
