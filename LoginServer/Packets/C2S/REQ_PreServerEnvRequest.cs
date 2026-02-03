using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_PreServerEnvRequest : PacketC2S<Client>
	{
		public REQ_PreServerEnvRequest(Queue<byte> data) : base((UInt16)Opcode.PRESERVERENVREQUEST, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			string username;

			try
			{
				username = PacketReader.ReadString(_data, 65);
				PacketReader.ReadDiscard(_data, 64);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnPreServerEnvRequest(x, username));

			return true;
		}
	}
}
