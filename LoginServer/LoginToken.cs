using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
	internal class LoginToken
	{
		internal byte[] Token { private set; get; }
		internal byte[] UserNameBytes { private set; get; }
		internal byte[] TimeBytes { private set; get; }
		internal byte[] IpBytes { private set; get; }


		public LoginToken(string username, long time, byte[] ip, string loginSecret)
		{
			SHA256 mySHA256 = SHA256.Create();
			UserNameBytes = Encoding.UTF8.GetBytes(username);
			TimeBytes = BitConverter.GetBytes(time);
			IpBytes = ip;
			var secretBytes = Encoding.UTF8.GetBytes(loginSecret);

			byte[] rv = new byte[UserNameBytes.Length + TimeBytes.Length + ip.Length + secretBytes.Length];
			System.Buffer.BlockCopy(UserNameBytes, 0, rv, 0, UserNameBytes.Length);
			System.Buffer.BlockCopy(TimeBytes, 0, rv, UserNameBytes.Length, TimeBytes.Length);
			System.Buffer.BlockCopy(ip, 0, rv, UserNameBytes.Length + TimeBytes.Length, ip.Length);
			System.Buffer.BlockCopy(secretBytes, 0, rv, UserNameBytes.Length + TimeBytes.Length + ip.Length, secretBytes.Length);

			Token = mySHA256.ComputeHash(rv);
			Debug.Assert(Token.Length == 32);
		}
	}
}
