using LibPegasus.Parsers.Mcl;
using Npgsql.TypeMapping;
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
		private Dictionary<int, TileAttributeData> _tileAttributes;

		public InstanceManager()
		{
			_instances = new Dictionary<UInt128, Instance>();
			_tileAttributes = new Dictionary<int, TileAttributeData>();

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

			instance.TileAttributeData = GetTileAttributeData((Int32)instance.MapId);

			_instances[instance.Id] = instance;
		}

		private TileAttributeData GetTileAttributeData(int mapId) //this needs to be async probably
		{
			if(_tileAttributes.ContainsKey(mapId))
			{
				return _tileAttributes[mapId];
			}
			else
			{
				var mapIdString = mapId.ToString("00");
				string workingDirectory = Environment.CurrentDirectory;
				string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
				var path = $"{projectDirectory}\\LibPegasus\\Data\\Maps\\mcl\\world_{mapIdString}.mcl";
				Serilog.Log.Information($"Loading map {path}");
				MclParser mclParser = new MclParser();
				mclParser.Parse(path);
				_tileAttributes.Add(mapId, mclParser.AttributeData);
				return _tileAttributes[mapId];
			}
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
