using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSystem.Security.Cryptography;

namespace Aesthetics.DataAccess.NetCore.CheckConditions
{
	public static class Security
	{
		public static string EncryptPassWord(string randomString)
		{
			var crypt = new SHA256Managed();
			string hash = String.Empty;
			byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
			foreach (byte theByte in crypto)
			{
				hash += theByte.ToString("x2");
			}
			return hash;
		}
	}
}
