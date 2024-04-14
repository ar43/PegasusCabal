using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Crypt
{
	public class XorKeyTable
	{
		public UInt32[] KeyTable { get; private set; }
		KeyRand _keyRand = new();

		public XorKeyTable()
		{
			KeyTable = new UInt32[Encryption.RecvXorKeyNum * 2];
			Generate();
		}
		private void Generate()
		{
			if (KeyTable == null)
				throw new NullReferenceException("GenerateXorKeyTable");

			_keyRand.Seed = Encryption.RecvXorSeed;

			for (int i = 0; i < Encryption.RecvXorKeyNum; i++)
			{
				var low = _keyRand.Next();
				var high = _keyRand.Next();
				KeyTable[i] = (UInt32)(low & 0xFFFF | (high & 0xFFFF) << 16);
			}

			_keyRand.Seed = Encryption.Recv2ndXorSeed;

			for (var i = Encryption.RecvXorKeyNum; i < Encryption.RecvXorKeyNum * 2; i++)
			{
				var low = _keyRand.Next();
				var high = _keyRand.Next();
				KeyTable[i] = (UInt32)(low & 0xFFFF | (high & 0xFFFF) << 16);
			}
		}

		

	}
}
