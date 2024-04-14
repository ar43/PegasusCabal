using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_Connect2Serv : PacketC2S<Client>
	{
		public REQ_Connect2Serv(Queue<byte> data) : base((UInt16)Opcode.CONNECT2SVR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			try
			{
				var reserved = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnServerConnection(x));

			return true;
		}

		public static UInt16 GetSize()
		{
			return 14;
		}
	}
}
