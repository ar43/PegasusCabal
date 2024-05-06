using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Logic.AccountData
{
	internal class Account
	{
		public Account(UInt32 id)
		{
			//TODO
			CashInventory = new CashInventory();
			Id = id;
		}

		public CashInventory? CashInventory { get; private set; }
		public UInt32 Id { get; private set; }
	}
}
