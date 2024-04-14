using LibPegasus.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibPegasus.Logic
{
	internal class ClientInfo
	{
		public static readonly int RSA_KEY_SIZE = 2048;

		public ClientInfo(UInt16 userId, UInt32 authKey)
		{
			UserId = userId;
			AuthKey = authKey;
			ConnState = ConnState.INITIAL;
			RSA = RSA.Create(RSA_KEY_SIZE);
			Username = "";
		}

		public UInt16 UserId { get; private set; }
		public UInt32 AuthKey { get; private set; }
		public string Username;
		public ConnState ConnState;
		public RSA RSA {  get; private set; }
	}
}
