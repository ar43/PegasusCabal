using LibPegasus.Packets;
using Nito.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.AccountData;

namespace WorldServer.Packets.S2C
{
	internal class NFY_DurationSvcData : PacketS2C
	{
		List<PremiumService> _premiumServices;
		public NFY_DurationSvcData(List<PremiumService> premiumServices) : base((UInt16)Opcode.CSC_DURATIONSVCDATA)
		{
			_premiumServices = premiumServices;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, 0);
			PacketWriter.WriteInt32(data, _premiumServices.Count);
			foreach (var service in _premiumServices)
			{
				PacketWriter.WriteUInt16(data, service.u1);
				PacketWriter.WriteUInt32(data, service.Idk);
				PacketWriter.WriteUInt32(data, service.Index);
				PacketWriter.WriteUInt32(data, service.Type);
				PacketWriter.WriteUInt32(data, service.EXP);
				PacketWriter.WriteUInt16(data, service.MinLvl);
				PacketWriter.WriteUInt16(data, service.MaxLvl);
				PacketWriter.WriteInt32(data, service.SkillEXP);
				PacketWriter.WriteInt32(data, service.DropRate);
				PacketWriter.WriteInt32(data, service.Craft);
				PacketWriter.WriteInt32(data, service.CraftSuccess);
				PacketWriter.WriteInt32(data, service.Inventory);
				PacketWriter.WriteInt32(data, service.Warehouse);
				PacketWriter.WriteInt32(data, service.AlzBomb);
				PacketWriter.WriteInt32(data, service.AtDummy);
				PacketWriter.WriteInt32(data, service.GPS);
				PacketWriter.WriteInt32(data, service.Inventory2);
				PacketWriter.WriteInt32(data, service.WEXP);
				PacketWriter.WriteInt32(data, service.PetEXP);
				PacketWriter.WriteByte(data, service.unk);
				PacketWriter.WriteUInt16(data, service.MarketSlot);
				PacketWriter.WriteUInt16(data, service.MarketStack);
				PacketWriter.WriteArray(data, service.Unknown);
				PacketWriter.WriteInt32(data, service.AXP);
				PacketWriter.WriteInt32(data, service.Point);
				PacketWriter.WriteInt32(data, service.EfDummy);
				PacketWriter.WriteArray(data, service.Unknown2);
			}
		}
	}
}
