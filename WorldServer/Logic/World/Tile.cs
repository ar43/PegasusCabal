using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.World
{
	internal class Tile
	{
		public Tile()
		{
			localClients = new();
		}

		public List<Client> localClients { get; private set; }
	}
}
