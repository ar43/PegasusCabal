using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.S2C.PacketSpecificData
{
	internal struct MobDamageResult
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

		public MobDamageResult(ObjectIndexData id)
		{
			ID = id;
		}
	}
}
