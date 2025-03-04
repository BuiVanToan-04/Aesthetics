using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
    public class ResponseUserSession : ResponseData
    {
		public int? UserSessionID { get; set; }
		public int? UserID { get; set; }
		public string? Token { get; set; }
		public string? DeviceName { get; set; }
		public string? Ip { get; set; }
		public DateTime? CreateTime { get; set; }
		public int? DeleteStatus { get; set; }
	}

	public class ResponseUserSession_Loggin : ResponseData
	{
		public List<ResponseUserSession> Data { get; set; }
		public List<UserSession_Loggin>? userSession_Loggins { get; set; }
	}
}
