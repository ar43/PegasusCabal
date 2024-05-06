using LibPegasus.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.Delegates;
using WorldServer.Logic;
using WorldServer.Enums;

namespace WorldServer.Packets.C2S
{
	internal class REQ_SubPasswordSet : PacketC2S<Client>
	{
		public REQ_SubPasswordSet(Queue<byte> data) : base((UInt16)Opcode.CSC_SUBPASSWORDSET, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			string subpass;
			SubPasswordType subpassType;
			UInt32 secretQuestion;
			string secretAnswer;
			SubPasswordLockType subpassLockType;

			try
			{
				subpass = PacketReader.ReadString(_data, 11);
				subpassType = (SubPasswordType)PacketReader.ReadUInt32(_data);
				secretQuestion = PacketReader.ReadUInt32(_data);
				secretAnswer = PacketReader.ReadString(_data, 128);
				subpassLockType = (SubPasswordLockType)PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => CharSelect.OnSubPasswordSet(x, subpass, subpassType, secretQuestion, secretAnswer, subpassLockType));

			return true;
		}
	}
}
