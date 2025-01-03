using LibPegasus.Packets;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Logic.CharData.Skills;
using WorldServer.Logic.Delegates;

namespace WorldServer.Packets.C2S
{
	internal class REQ_SkillToUser : PacketC2S<Client>
	{
		public REQ_SkillToUser(Queue<byte> data) : base((UInt16)Opcode.CSC_SKILLTOUSER, data)
		{
		}

		public override bool ReadPayload(Queue<Action<Client>> actions)
		{
			UInt16 skillId;
			Byte slot;
			UInt16 state;
			UInt32 flags;
			Skill temp;

			try
			{
				skillId = PacketReader.ReadUInt16(_data);
				try
				{
					temp = new(skillId, 1);
				}
				catch (KeyNotFoundException)
				{
					Serilog.Log.Error("Client sent a skill that doesnt exist on SkillToUser");
					return false;
				}
				if(temp.GetGroup() == SkillGroup.SK_GROUP031)
				{
					slot = PacketReader.ReadByte(_data);
					state = PacketReader.ReadUInt16(_data);
					flags = PacketReader.ReadUInt32(_data);
				}
				else
				{
					throw new NotImplementedException("research for different skill groups, packet differs");
				}
			}
			catch (IndexOutOfRangeException)
			{
				return false;
			}

			switch(temp.GetGroup())
			{
				case SkillGroup.SK_GROUP031:
					actions.Enqueue((client) => CharAction.OnSkillToUserG31(client, skillId, slot, state, flags));
					break;
				default:
					throw new NotImplementedException("research for different skill groups, packet differs");
			}
			

			return true;
		}
	}
}
