using LibPegasus.Packets;
using Nito.Collections;
using WorldServer.Enums;
using WorldServer.Logic.WorldRuntime.InstanceRuntime;

namespace WorldServer.Packets.S2C
{
	internal class NFY_NewMobsList : PacketS2C
	{
		List<Mob> _mobs;
		public NFY_NewMobsList(List<Mob> mobs) : base((UInt16)Opcode.NFY_NEWMOBSLIST)
		{
			_mobs = mobs;
		}

		public override void WritePayload(Deque<byte> data)
		{
			PacketWriter.WriteByte(data, (Byte)_mobs.Count);
			foreach (var mob in _mobs)
			{
				if (!mob.IsSpawned)
					throw new Exception("Sending data about unspawned mob");
				PacketWriter.WriteUInt16(data, mob.ObjectIndexData.ObjectId);
				PacketWriter.WriteByte(data, mob.ObjectIndexData.WorldIndex);
				PacketWriter.WriteByte(data, (Byte)mob.ObjectIndexData.ObjectType);
				PacketWriter.WriteUInt16(data, (UInt16)mob.X); //from
				PacketWriter.WriteUInt16(data, (UInt16)mob.Y); //from
				PacketWriter.WriteUInt16(data, (UInt16)mob.X); //to
				PacketWriter.WriteUInt16(data, (UInt16)mob.Y); //to
				PacketWriter.WriteUInt16(data, mob.GetSpecies());
				PacketWriter.WriteInt32(data, mob.GetMaxHP());
				PacketWriter.WriteInt32(data, mob.HP);
				PacketWriter.WriteBool(data, mob.IsChasing);
				PacketWriter.WriteByte(data, mob.Level);
				PacketWriter.WriteByte(data, mob.Nation);
				PacketWriter.WriteInt32(data, 0); //unknown
				PacketWriter.WriteInt16(data, 0); //unknown
			}
		}
	}
}
