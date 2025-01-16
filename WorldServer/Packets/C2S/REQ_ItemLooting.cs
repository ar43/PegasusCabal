using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;
using WorldServer.Logic.SharedData;

namespace WorldServer.Packets.C2S
{
	internal class REQ_ItemLooting : PacketC2S<Client>
	{
		public REQ_ItemLooting(Queue<byte> data) : base((UInt16)Opcode.CSC_ITEMLOOTING, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 id;
			Byte worldIndex;
			Byte objectType;

			UInt16 key;
			UInt32 itemKind;
			UInt16 slot;

			try
			{
				id = PacketReader.ReadUInt16(_data);
				worldIndex = PacketReader.ReadByte(_data);
				objectType = PacketReader.ReadByte(_data);

				key = PacketReader.ReadUInt16(_data);
				itemKind = PacketReader.ReadUInt32(_data);
				slot = PacketReader.ReadUInt16(_data);
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			actions.Enqueue((client) => CharAction.OnItemLoot(client, new ObjectIndexData(id, worldIndex, (ObjectType)objectType), key, itemKind, slot));

			return true;
		}
	}
}
