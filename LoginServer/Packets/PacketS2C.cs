using LoginServer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginServer.Crypt;
using Nito.Collections;

namespace LoginServer.Opcodes
{
	internal class PacketS2C
	{
		private readonly Opcode id;
		protected Deque<byte> _data = new();

		public static readonly UInt16 HEADER_SIZE = 6;

		public PacketS2C(Opcode id)
		{
			this.id = id;
		}

		private void WriteHeader()
		{
			// in reverse order
			var size = (UInt16)(HEADER_SIZE + _data.Count);
			PacketWriter.WriteHeader(_data, (UInt16)id);
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
