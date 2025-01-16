using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.C2S.PacketSpecificData
{
	internal struct MobTarget
	{
		public ObjectIndexData Id { get; private set; }
		public Byte Type { get; private set; }
		public Byte U3 { get; private set; }
		public Byte U4 { get; private set; }

		public MobTarget(ObjectIndexData id, Byte type, Byte u3, Byte u4)
		{
			Id = id;
			Type = type;
			U3 = u3;
			U4 = u4;
		}
	}
}
