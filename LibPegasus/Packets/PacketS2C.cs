using LibPegasus.Crypt;
using Nito.Collections;

namespace LibPegasus.Packets
{
	public class PacketS2C
	{
		private readonly UInt16 _id;

		public static readonly UInt16 HEADER_SIZE = 6;

		public PacketS2C(UInt16 id)
		{
			this._id = id;
		}

		private void WriteHeader(Deque<byte> data)
		{
			// in reverse order
			var size = (UInt16)(HEADER_SIZE + data.Count);
			PacketWriter.WriteHeader(data, (UInt16)_id);
			PacketWriter.WriteHeader(data, size);
			PacketWriter.WriteHeader(data, Encryption.MagicKey);
		}

		public Deque<byte> Send()
		{
			Deque<byte> data = new();
			WritePayload(data);
			WriteHeader(data);
			return data;
		}

		public virtual void WritePayload(Deque<byte> data)
		{
			throw new NotImplementedException();
		}
	}
}
