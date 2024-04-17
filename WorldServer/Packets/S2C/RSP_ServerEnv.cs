using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_ServerEnv : PacketS2C
	{
		Gamesettings _gamesettings;
		public RSP_ServerEnv(Gamesettings gamesettings) : base((UInt16)Opcode.SERVERENV)
		{
			_gamesettings = gamesettings;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteUInt16(_data, (UInt16)_gamesettings.MaxLevel);
			PacketWriter.WriteBool(_data, _gamesettings.DummyEnabled);
			PacketWriter.WriteBool(_data, _gamesettings.CashShopEnabled);
			PacketWriter.WriteBool(_data, _gamesettings.NetcafePointsEnabled);
			PacketWriter.WriteUInt16(_data, (UInt16)_gamesettings.MaxRank);
			PacketWriter.WriteUInt16(_data, (UInt16)_gamesettings.LimitLoudCharLv);
			PacketWriter.WriteUInt16(_data, (UInt16)_gamesettings.LimitLoudMasteryLv);
			PacketWriter.WriteUInt64(_data, (UInt64)_gamesettings.LimitInvAlzSave);
			PacketWriter.WriteUInt64(_data, (UInt64)_gamesettings.LimitWhAlzSave);
			PacketWriter.WriteUInt64(_data, (UInt64)_gamesettings.LimitTradeAlz);
			PacketWriter.WriteBool(_data, _gamesettings.AllowDuplicatedPCBangPremium);
			PacketWriter.WriteBool(_data, _gamesettings.GuildBoardEnabled);
			PacketWriter.WriteByte(_data, (Byte)_gamesettings.PCBangPremiumPrioType);
			PacketWriter.WriteInt32(_data, _gamesettings.UseTradeChannelRestriction);
			PacketWriter.WriteBool(_data, _gamesettings.AgentShopEnabled);
			PacketWriter.WriteNull(_data, 3);
			PacketWriter.WriteUInt16(_data, (UInt16)_gamesettings.UseLordBroadCastCoolTimeSec);
			PacketWriter.WriteByte(_data, (Byte)_gamesettings.DummyLimitLv);
			PacketWriter.WriteInt16(_data, (Int16)_gamesettings.AgentShopRestrictionLv);
			PacketWriter.WriteInt16(_data, (Int16)_gamesettings.PersonalShopRestrictionLv);
			PacketWriter.WriteBool(_data, _gamesettings.UseTPoint);
			PacketWriter.WriteBool(_data, _gamesettings.UseGuildExpansion);
			PacketWriter.WriteBool(_data, _gamesettings.IgnorePartyInviteDistance);
			PacketWriter.WriteBool(_data, _gamesettings.LimitedBroadCastByLord);
			PacketWriter.WriteByte(_data, (Byte)_gamesettings.LimitNormalChatLev);
			PacketWriter.WriteByte(_data, (Byte)_gamesettings.LimitTradeChatLev);
			PacketWriter.WriteUInt32(_data, (UInt32)_gamesettings.MaxDPLimit);
			PacketWriter.WriteNull(_data, 4+2);
		}
	}
}
