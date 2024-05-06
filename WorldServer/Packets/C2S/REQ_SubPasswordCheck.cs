using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;

namespace WorldServer.Packets.C2S
{
	internal class REQ_SubPasswordCheck : PacketC2S<Client>
	{
		public REQ_SubPasswordCheck(Queue<byte> data) : base((UInt16)Opcode.CSC_SUBPASSWORDCHECK, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			string subpass;
			SubPasswordType subpassType;
			byte hours;
			SubPasswordLockType subpassLockType;

			try
			{
				subpass = PacketReader.ReadString(_data, 11);
				subpassType = (SubPasswordType)PacketReader.ReadUInt32(_data);
				hours = PacketReader.ReadByte(_data);
				subpassLockType = (SubPasswordLockType)PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => CharSelect.OnSubPasswordCheck(x, subpass, subpassType, hours, subpassLockType));

			return true;
		}
	}
}
