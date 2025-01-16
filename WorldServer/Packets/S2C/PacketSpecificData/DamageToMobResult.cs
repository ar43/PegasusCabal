using WorldServer.Enums;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.S2C.PacketSpecificData
{
	internal struct DamageToMobResult
	{
		public ObjectIndexData ID;
		public byte Type;
		public AttackResult AttackResult;
		public ushort DamageReceived;
		public uint HPLeft;
		public uint u8 = 0;
		public uint u9 = 0;
		public byte u11 = 0;
		public byte HasBFX;

		public DamageToMobResult(ObjectIndexData id)
		{
			ID = id;
		}
	}
}
