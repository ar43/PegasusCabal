using LibPegasus.Packets;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_ChargeInfo : PacketS2C
	{
		Int32 _payMode, _remainingTime, _serviceKind;
		public NFY_ChargeInfo(Int32 payMode, Int32 remainingTime, Int32 serviceKind) : base((UInt16)Opcode.NFY_CHARGEINFO)
		{
			_payMode = payMode;
			_remainingTime = remainingTime;
			_serviceKind = serviceKind;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteInt32(_data, _payMode);
			PacketWriter.WriteInt32(_data, _remainingTime);
			PacketWriter.WriteInt32(_data, _serviceKind);
		}
	}
}
