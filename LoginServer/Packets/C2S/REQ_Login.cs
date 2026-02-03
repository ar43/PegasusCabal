using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_Login : PacketC2S<Client>
	{
		public REQ_Login(Queue<byte> data) : base((UInt16)Opcode.LOGIN, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte[] rsaData;

			try
			{
				PacketReader.ReadDiscard(_data, 4);
				rsaData = PacketReader.ReadArray(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnLogin(x, rsaData));

			return true;
		}
	}
}
