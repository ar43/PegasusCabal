using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_UpdateHelpInfo : PacketC2S<Client>
	{
		public REQ_UpdateHelpInfo(Queue<byte> data) : base((UInt16)Opcode.CSC_UPDATEHELPINFO, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt32 unknown;

			try
			{
				unknown = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => Interface.OnUpdateHelpInfo(client));

			return true;
		}
	}
}
