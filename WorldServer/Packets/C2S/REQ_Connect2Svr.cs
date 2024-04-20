using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_Connect2Svr : PacketC2S<Client>
	{
		public REQ_Connect2Svr(Queue<byte> data) : base((UInt16)Opcode.CONNECT2SVR, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			byte serverId;
			byte channelId;
			byte u0;
			byte u1;
			try
			{
				serverId = PacketReader.ReadByte(_data);
				channelId = PacketReader.ReadByte(_data);
				u0 = PacketReader.ReadByte(_data);
				u1 = PacketReader.ReadByte(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((x) => Connection.OnServerConnection(x, serverId, channelId));

			return true;
		}

		public static UInt16 GetSize()
		{
			return 14;
		}
	}
}
