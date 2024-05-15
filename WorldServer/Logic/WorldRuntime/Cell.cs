using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.WorldRuntime
{
	internal class Cell
	{
		public Cell()
		{
			LocalClients = new();
		}

		public HashSet<Client> LocalClients { get; private set; }
	}
}
