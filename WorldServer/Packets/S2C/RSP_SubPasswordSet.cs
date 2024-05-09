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
	internal class RSP_SubPasswordSet : PacketS2C
	{
		private UInt32 _result;
		private UInt32 _changePassword;
		private SubPasswordType _subPasswordType;
		private SubPasswordLockType _subPasswordLockType;

		public RSP_SubPasswordSet(UInt32 result, UInt32 changePassword, SubPasswordType subPasswordType, SubPasswordLockType subPasswordLockType) : base((UInt16)Opcode.CSC_SUBPASSWORDSET)
		{
			_result = result;
			_changePassword = changePassword;
			_subPasswordType = subPasswordType;
			_subPasswordLockType = subPasswordLockType;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _result);
			PacketWriter.WriteUInt32(data, _changePassword);
			PacketWriter.WriteUInt32(data, (UInt32)_subPasswordType);
			PacketWriter.WriteUInt32(data, (UInt32)_subPasswordLockType);
		}
	}
}
