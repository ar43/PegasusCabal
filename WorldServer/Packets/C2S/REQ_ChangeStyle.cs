using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.CharData;
using WorldServer.Logic.CharData.Styles;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
    internal class REQ_ChangeStyle : PacketC2S<Client>
	{
		public REQ_ChangeStyle(Queue<byte> data) : base((UInt16)Opcode.CSC_CHANGESTYLE, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			Style newStyle;
			LiveStyle newLiveStyle;
			BuffFlag newBuffFlag;
			ActionFlag newActionFlag;
			try
			{
				newStyle = new Style(PacketReader.ReadUInt32(_data));
				newLiveStyle = new LiveStyle(PacketReader.ReadUInt32(_data));
				newBuffFlag = new BuffFlag(PacketReader.ReadUInt32(_data));
				newActionFlag = new ActionFlag(PacketReader.ReadUInt16(_data));
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharAction.OnChangeStyle(client, newStyle, newLiveStyle, newBuffFlag, newActionFlag));

			return true;
		}
	}
}
