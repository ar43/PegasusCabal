using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;
using WorldServer.Logic.CharData;
using WorldServer.Packets.S2C;

namespace WorldServer.Logic.World
{
	internal class InstanceManager
	{
		private Dictionary<UInt128, Instance> _instances;

		public InstanceManager()
		{
			_instances = new Dictionary<UInt128, Instance>();

			AddInstance(new Instance(Enums.MapId.BLOODY_ICE, InstanceType.PERMANENT));
			AddInstance(new Instance(Enums.MapId.GREEN_DESPAIR, InstanceType.PERMANENT));
			AddInstance(new Instance(Enums.MapId.DESERT_SCREAM, InstanceType.PERMANENT));
		}

		public void AddInstance(Instance instance)
		{
			if(_instances.ContainsKey(instance.Id))
			{
				throw new Exception("Instance manager already contains and instance with that id");
			}

			_instances[instance.Id] = instance;
		}

		public void AddClient(Client client, UInt128 instanceId)
		{
			_instances[instanceId].AddNewClient(client, (UInt16)client.Character.Location.Movement.CellX, (UInt16)client.Character.Location.Movement.CellY);
			client.Character.Location.Instance = _instances[instanceId];

			var otherCharacters = client.Character.Location.Instance.GetNearbyCharacters(client);
			if (otherCharacters.Count > 0)
			{
				var packet_new_list_others = new NFY_NewUserList(otherCharacters, NewUserType.OTHERPLAYERS);
				client.PacketManager.Send(packet_new_list_others);
			}

			var packet_new_list_this = new NFY_NewUserList(new List<Character>() { client.Character }, NewUserType.NEWINIT);
			client.BroadcastNearby(packet_new_list_this, true);
		}
	}
}
