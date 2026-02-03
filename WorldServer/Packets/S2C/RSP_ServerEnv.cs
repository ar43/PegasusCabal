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
			PacketWriter.WriteBool(data, _gamesettings.AgentShopEnabled);
			PacketWriter.WriteNull(data, 3);
			PacketWriter.WriteByte(data, 0);
			PacketWriter.WriteByte(data, (Byte)_gamesettings.DummyLimitLv);
			PacketWriter.WriteInt16(data, (Int16)_gamesettings.AgentShopRestrictionLv);
			PacketWriter.WriteInt16(data, (Int16)_gamesettings.PersonalShopRestrictionLv);
			PacketWriter.WriteBool(data, _gamesettings.UseTPoint);
			PacketWriter.WriteBool(data, _gamesettings.UseGuildExpansion);
			PacketWriter.WriteByte(data, 0);
			PacketWriter.WriteBool(data, _gamesettings.LimitedBroadCastByLord);
			PacketWriter.WriteByte(data, (Byte)_gamesettings.LimitNormalChatLev);
			PacketWriter.WriteByte(data, (Byte)_gamesettings.LimitTradeChatLev);
			PacketWriter.WriteUInt32(data, (UInt32)_gamesettings.MaxDPLimit);
			PacketWriter.WriteInt32(data, 1000000000);
			PacketWriter.WriteInt32(data, 7);
			PacketWriter.WriteByte(data, 1);
			PacketWriter.WriteNull(data, 3);
			PacketWriter.WriteUInt64(data, 4000000000);
			PacketWriter.WriteUInt64(data, Convert.ToUInt64(-2000000000));
			PacketWriter.WriteInt32(data, 1);
			PacketWriter.WriteInt32(data, 16);
			PacketWriter.WriteInt32(data, 15);
			PacketWriter.WriteInt32(data, 2);
			PacketWriter.WriteInt32(data, 109);
			PacketWriter.WriteInt32(data, 110);
			PacketWriter.WriteInt32(data, 3);
			PacketWriter.WriteInt32(data, 158);
			PacketWriter.WriteInt32(data, 159);
			PacketWriter.WriteNull(data, 255);
			PacketWriter.WriteByte(data, 5);
			PacketWriter.WriteNull(data, 15);
			PacketWriter.WriteByte(data, 1);
		}
	}
}
