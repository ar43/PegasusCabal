using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;

namespace WorldServer.Packets.S2C
{
	internal class RSP_ServerEnv : PacketS2C
	{
		Gamesettings _gamesettings;
		public RSP_ServerEnv(Gamesettings gamesettings) : base((UInt16)Opcode.CSC_SERVERENV)
		{
			_gamesettings = gamesettings;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteUInt16(data, (UInt16)_gamesettings.MaxLevel);
			PacketWriter.WriteBool(data, _gamesettings.DummyEnabled);
			PacketWriter.WriteBool(data, _gamesettings.CashShopEnabled);
			PacketWriter.WriteBool(data, _gamesettings.NetcafePointsEnabled);
			PacketWriter.WriteUInt16(data, (UInt16)_gamesettings.MaxRank);
			PacketWriter.WriteUInt16(data, (UInt16)_gamesettings.LimitLoudCharLv);
			PacketWriter.WriteUInt16(data, (UInt16)_gamesettings.LimitLoudMasteryLv);
			PacketWriter.WriteUInt64(data, (UInt64)_gamesettings.LimitInvAlzSave);
			PacketWriter.WriteUInt64(data, (UInt64)_gamesettings.LimitWhAlzSave);
			PacketWriter.WriteUInt64(data, (UInt64)_gamesettings.LimitTradeAlz);
			PacketWriter.WriteBool(data, _gamesettings.AllowDuplicatedPCBangPremium);
			PacketWriter.WriteBool(data, _gamesettings.GuildBoardEnabled);
			PacketWriter.WriteByte(data, (Byte)_gamesettings.PCBangPremiumPrioType);
			PacketWriter.WriteInt32(data, _gamesettings.UseTradeChannelRestriction);
			PacketWriter.WriteBool(data, _gamesettings.AgentShopEnabled);
			PacketWriter.WriteNull(data, 3);
			PacketWriter.WriteUInt16(data, (UInt16)_gamesettings.UseLordBroadCastCoolTimeSec);
			PacketWriter.WriteByte(data, (Byte)_gamesettings.DummyLimitLv);
			PacketWriter.WriteInt16(data, (Int16)_gamesettings.AgentShopRestrictionLv);
			PacketWriter.WriteInt16(data, (Int16)_gamesettings.PersonalShopRestrictionLv);
			PacketWriter.WriteBool(data, _gamesettings.UseTPoint);
			PacketWriter.WriteBool(data, _gamesettings.UseGuildExpansion);
			PacketWriter.WriteBool(data, _gamesettings.IgnorePartyInviteDistance);
			PacketWriter.WriteBool(data, _gamesettings.LimitedBroadCastByLord);
			PacketWriter.WriteByte(data, (Byte)_gamesettings.LimitNormalChatLev);
			PacketWriter.WriteByte(data, (Byte)_gamesettings.LimitTradeChatLev);
			PacketWriter.WriteUInt32(data, (UInt32)_gamesettings.MaxDPLimit);
			PacketWriter.WriteNull(data, 4 + 2);
		}
	}
}
