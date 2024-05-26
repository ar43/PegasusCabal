using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_Initialized : PacketC2S<Client>
	{
		public REQ_Initialized(Queue<byte> data) : base((UInt16)Opcode.CSC_INITIALIZED, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt32 charId;

			try
			{
				charId = PacketReader.ReadUInt32(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharSelect.OnInitialize(client, charId));

			return true;
		}
	}
}
