using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.Delegates;
using WorldServer.Logic.SharedData;
using WorldServer.Packets.C2S.PacketSpecificData;

namespace WorldServer.Packets.C2S
{
	internal class REQ_SkillToMobs : PacketC2S<Client>
	{
		public REQ_SkillToMobs(Queue<byte> data) : base((UInt16)Opcode.CSC_SKILLTOMOBS, data)
		{

		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 skillId;
			Byte slot;
			UInt32 u0;
			UInt16 x, y;
			Byte u1;
			UInt32 u2;
			Byte mobAmount;
			List<MobTarget> mobs = new();

			try
			{
				skillId = PacketReader.ReadUInt16(_data);
				slot = PacketReader.ReadByte(_data);
				u0 = PacketReader.ReadUInt32(_data);
				x = PacketReader.ReadUInt16(_data);
				y = PacketReader.ReadUInt16(_data);
				u1 = PacketReader.ReadByte(_data);
				u2 = PacketReader.ReadUInt32(_data);
				mobAmount = PacketReader.ReadByte(_data);
				for(int i = 0; i < mobAmount; i++)
				{
					var mobId = PacketReader.ReadUInt16(_data);
					var worldIndex = PacketReader.ReadByte(_data);
					var objectType = PacketReader.ReadByte(_data);
					ObjectIndexData id = new(mobId, worldIndex, (ObjectType)objectType);
					var type = PacketReader.ReadByte(_data);
					var u4 = PacketReader.ReadByte(_data);
					var u5 = PacketReader.ReadByte(_data);
					mobs.Add(new MobTarget(id, type, u4, u5));
				}
			}
			catch (Exception ex)
			{
				Serilog.Log.Error(ex.Message);
				return false;
			}

			actions.Enqueue((client) => Battle.OnSkillToMobs(client, skillId, slot, u0, x, y, u1, u2, mobs));

			return true;
		}
	}
}
