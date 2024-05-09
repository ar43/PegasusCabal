using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_Connect2Svr : PacketS2C
	{
		private UInt32 _seed2nd;
		private UInt32 _authKey;
		private UInt16 _userIdx;
		private UInt16 _recvXorKeyIdx;

		public RSP_Connect2Svr(UInt32 seed2nd, UInt32 authKey, UInt16 userIdx, UInt16 recvXorKeyIdx) : base((UInt16)Opcode.CSC_CONNECT2SVR)
		{
			_seed2nd = seed2nd;
			_authKey = authKey;
			_userIdx = userIdx;
			_recvXorKeyIdx = recvXorKeyIdx;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt32(data, _seed2nd); // should prolly be a random value?
			PacketWriter.WriteUInt32(data, _authKey);
			PacketWriter.WriteUInt16(data, _userIdx);
			PacketWriter.WriteUInt16(data, _recvXorKeyIdx);
			//Serilog.Log.Debug($"Expecting to see {_recvXorKeyIdx}");
		}
	}
}
