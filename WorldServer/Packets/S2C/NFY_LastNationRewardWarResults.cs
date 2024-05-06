using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class NFY_LastNationRewardWarResults : PacketS2C
	{
		Int32 _resultId, _totalRounds, _capellaWins, _procyonWinds;
		DateTimeOffset _rewardStartDate, _rewardEndDate;
		byte _warMapid;

		public NFY_LastNationRewardWarResults(Int32 resultId, Int32 totalRounds, Int32 capellaWins, Int32 procyonWinds, DateTimeOffset rewardStartDate, DateTimeOffset rewardEndDate, Byte warMapid) : base((UInt16)Opcode.NFY_LASTNATIONREWARDWARRESULTS)
		{
			_resultId = resultId;
			_totalRounds = totalRounds;
			_capellaWins = capellaWins;
			_procyonWinds = procyonWinds;
			_rewardStartDate = rewardStartDate;
			_rewardEndDate = rewardEndDate;
			_warMapid = warMapid;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteInt32(_data, _resultId);
			PacketWriter.WriteInt32(_data, _totalRounds);
			PacketWriter.WriteInt32(_data, _capellaWins);
			PacketWriter.WriteInt32(_data, _procyonWinds);
			PacketWriter.WriteUInt64(_data, (UInt64)_rewardStartDate.ToUnixTimeSeconds());
			PacketWriter.WriteUInt64(_data, (UInt64)_rewardEndDate.ToUnixTimeSeconds());
			PacketWriter.WriteInt32(_data, 0); //unknown
			PacketWriter.WriteByte(_data, _warMapid);
		}
	}
}
