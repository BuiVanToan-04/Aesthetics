using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.CheckConditions.Response
{
	public class UserLoginResponseData : ResponseData
	{
		public string Token { get; set; }

		public string RefreshToken { get; set; }

		public int UserID { get; set; }

		public string DeviceName { get; set; }
	}

	public class CheckTokenReponsData : ResponseData 
	{
		public string Token { get; set; }

		public string RefreshToken { get; set; }
	}

	public class UserLogOutResponseData : ResponseData
	{

	}
}
