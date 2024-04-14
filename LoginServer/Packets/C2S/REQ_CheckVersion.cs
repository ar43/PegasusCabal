using LibPegasus.Packets;
using LoginServer.Enums;
using LoginServer.Logic;
using LoginServer.Logic.Delegates;

namespace LoginServer.Packets.C2S
{
	internal class REQ_CheckVersion : PacketC2S<Client>
	{
		public REQ_CheckVersion(Queue<byte> data) : base((UInt16)Opcode.CHECKVERSION, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt32 clientVersion;

			try
			{
				clientVersion = PacketReader.ReadUInt32(_data);
				PacketReader.ReadDiscard(_data, 4 * 3);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnCheckVersion(x, clientVersion));

			return true;
		}
	}
}
