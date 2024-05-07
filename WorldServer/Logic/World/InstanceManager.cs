using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Enums;

namespace WorldServer.Logic.World
{
	internal class InstanceManager
	{
		private Dictionary<UInt128, Instance> _instances;

		public InstanceManager()
		{
			_instances = new Dictionary<UInt128, Instance>();

			AddInstance(new Instance(Enums.WorldId.BLOODY_ICE, InstanceType.PERMANENT));
			AddInstance(new Instance(Enums.WorldId.GREEN_DESPAIR, InstanceType.PERMANENT));
			AddInstance(new Instance(Enums.WorldId.DESERT_SCREAM, InstanceType.PERMANENT));
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
			_instances[instanceId].AddClient(client, client.Character.Location.TileX, client.Character.Location.TileY);
			client.Character.Location.Instance = _instances[instanceId];
		}
	}
}
