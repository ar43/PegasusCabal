using WorldServer.Enums;

namespace WorldServer.Packets.S2C.PacketSpecificData
{
	internal struct DamageFromMobResult
	{
		public int CharId;
		public bool IsDead;
		public AttackResult AttackResult;
		public ushort Damage;
		public int RemainingHp;
		byte[] unknown;

		public DamageFromMobResult()
		{
			unknown = new byte[15];
		}
	}
}
