using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_AuthAccount : PacketC2S<Client>
	{
		public REQ_AuthAccount(Queue<byte> data) : base((UInt16)Opcode.AUTHACCOUNT, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte[] rsaData;

			try
			{
				PacketReader.ReadDiscard(_data, 2);
				rsaData = PacketReader.ReadArray(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnAuthAccount(x, rsaData));

			return true;
		}
	}
}
