using LibPegasus.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace LibPegasus.Packets
{
    public class PacketC2S<ClientClass>
	{
		private readonly UInt16 _id;
		protected readonly int _packetLen;
		protected Queue<byte> _data;

		public static readonly UInt16 HEADER_SIZE = 10;

		public PacketC2S(UInt16 id, Queue<byte> data)
		{
			_id = id;
			_data = data;
			_packetLen = data.Count;
		}

		public bool ReadHeader()
		{
			try
			{
				var magicKey = PacketReader.ReadUInt16(_data);
				var len = PacketReader.ReadUInt16(_data);
				var checksum = PacketReader.ReadUInt32(_data);
				var opcode = PacketReader.ReadUInt16(_data);

				if(magicKey != Encryption.MagicKey)
				{
					Log.Error("MagicKey does not match");
					return false;
				}

				if(len != _packetLen)
				{
					Log.Error("packet len does not match");
					return false;
				}

				if(opcode != (UInt16)_id)
				{
					Log.Error($"opcode does not match: got {opcode} expected {_id}");
					return false;
				}
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			return true;
		}

		public bool Verify()
		{
			return _data.Count == 0;
		}

		public virtual bool ReadPayload(Queue<Action<ClientClass>> actions)
		{
			throw new NotImplementedException();
		}
	}
}
