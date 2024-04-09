using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginServer.Logic;

namespace LoginServer.Packets
{
    internal class DanglingPacket
	{
		public byte[] DanglingData = new byte[Client.MAX_C2S_PACKET_LEN];
		public int PacketLen;
		int _currentPosition = 0;
		public bool Resolved = false;

		public DanglingPacket(byte[] data, int i, int length, int packetLen)
		{
			if((length-i) >= DanglingData.Length)
				throw new OverflowException("DanglingPacket: length-i >= Client.MAX_C2S_PACKET_LEN");

			Array.Copy(data, i, DanglingData, _currentPosition, length-i);
			_currentPosition += length - i;
			PacketLen = packetLen;
		}

		public int GetRemaining()
		{
			return PacketLen - _currentPosition;
		}

		public int Add(byte[] data, int amountToCopy)
		{
			Array.Copy(data, 0, DanglingData, _currentPosition, amountToCopy);

			_currentPosition += amountToCopy;

			return GetRemaining();
		}
	}
}
