using LibPegasus.Enums;
using LibPegasus.Packets;
using LoginServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets.S2C
{
	internal class NFY_SystemMessg : PacketS2C
	{
		MessageType _msgType;
		string _msg;
		public NFY_SystemMessg(MessageType msgType, String msg) : base((UInt16)Opcode.SYSTEMMESSG)
		{
			_msgType = msgType;
			_msg = msg;

			_msg ??= "";
		}

		public override void WritePayload()
		{
			PacketWriter.WriteByte(_data, (byte)_msgType);
			PacketWriter.WriteUInt16(_data, (UInt16)_msg.Length);
			if(_msg.Length > 0)
			{
				PacketWriter.WriteArray(_data, Encoding.ASCII.GetBytes(_msg));
				PacketWriter.WriteByte(_data, 0); //possibly need a null byte here or perhaps not
			}
		}
	}
}
