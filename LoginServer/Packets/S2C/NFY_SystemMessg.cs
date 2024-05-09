using LibPegasus.Packets;
using LoginServer.Enums;
using Nito.Collections;
using System.Text;

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

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, (byte)_msgType);
			PacketWriter.WriteUInt16(data, (UInt16)_msg.Length);
			if (_msg.Length > 0)
			{
				PacketWriter.WriteArray(data, Encoding.ASCII.GetBytes(_msg));
				PacketWriter.WriteByte(data, 0); //possibly need a null byte here or perhaps not
			}
		}
	}
}
