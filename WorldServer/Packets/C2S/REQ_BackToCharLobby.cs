using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.CharData;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_BackToCharLobby : PacketC2S<Client>
	{
		public REQ_BackToCharLobby(Queue<byte> data) : base((UInt16)Opcode.CSC_BACKTOCHARLOBBY, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			try
			{
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => IngameConnection.OnBackToCharLobby(client));

			return true;
		}
	}
}
