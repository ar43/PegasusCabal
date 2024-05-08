using LibPegasus.Packets;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_DelUserList : PacketS2C
	{
		Int32 _charId;
		DelUserType _delUserType;

		public NFY_DelUserList(Int32 charId, DelUserType delUserType) : base((UInt16)Opcode.NFY_DELUSERLIST)
		{
			_charId = charId;
			_delUserType = delUserType;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteInt32(_data, _charId);
			PacketWriter.WriteByte(_data, (byte)_delUserType);
		}
	}
}
