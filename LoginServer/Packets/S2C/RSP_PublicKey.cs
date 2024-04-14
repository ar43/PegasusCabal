using LibPegasus.Packets;
using LoginServer.Enums;

namespace LoginServer.Packets.S2C
{
	internal class RSP_PublicKey : PacketS2C
	{
		private byte[] _key;

		public RSP_PublicKey(byte[] key) : base((UInt16)Opcode.PUBLICKEY)
		{
			_key = key;
		}

		public override void WritePayload()
		{
			PacketWriter.WriteByte(_data, 1);
			PacketWriter.WriteUInt16(_data, (ushort)_key.Length);
			PacketWriter.WriteArray(_data, _key);
		}
	}
}
