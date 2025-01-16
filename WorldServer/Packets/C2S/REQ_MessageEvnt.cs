using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_MessageEvnt : PacketC2S<Client>
	{
		public REQ_MessageEvnt(Queue<byte> data) : base((UInt16)Opcode.REQ_MESSAGEEVNT, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 messageBlockLength, messageLength;
			byte data1, data2;
			MsgType msgType;
			string message;
			byte memoCount;
			byte itemCount;
			byte u2;

			try
			{
				messageBlockLength = PacketReader.ReadUInt16(_data);
				messageLength = PacketReader.ReadUInt16(_data);
				if (messageLength > 153)
					return false;
				data1 = PacketReader.ReadByte(_data);
				data2 = PacketReader.ReadByte(_data);
				msgType = (MsgType)PacketReader.ReadByte(_data);
				message = PacketReader.ReadString(_data, messageLength - 3);
				memoCount = PacketReader.ReadByte(_data);
				if (memoCount != 0)
					throw new NotImplementedException();
				itemCount = PacketReader.ReadByte(_data);
				if (itemCount != 0)
					throw new NotImplementedException();
				u2 = PacketReader.ReadByte(_data);
				if (u2 != 0)
					throw new NotImplementedException();
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => WorldChat.OnLocalMessageRequest(client, msgType, message));

			return true;
		}
	}
}
