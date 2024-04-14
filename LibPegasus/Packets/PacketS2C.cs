using LibPegasus.Crypt;
using Nito.Collections;

namespace LibPegasus.Packets
{
	public class PacketS2C
	{
		private readonly UInt16 _id;
		protected Deque<byte> _data = new();

		public static readonly UInt16 HEADER_SIZE = 6;

		public PacketS2C(UInt16 id)
		{
			this._id = id;
		}

		private void WriteHeader()
		{
			// in reverse order
			var size = (UInt16)(HEADER_SIZE + _data.Count);
			PacketWriter.WriteHeader(_data, (UInt16)_id);
			PacketWriter.WriteHeader(_data, size);
			PacketWriter.WriteHeader(_data, Encryption.MagicKey);
		}

		public Deque<byte> Send()
		{
			WritePayload();
			WriteHeader();
			return _data;
		}

		public virtual void WritePayload()
		{
			throw new NotImplementedException();
		}
	}
}
