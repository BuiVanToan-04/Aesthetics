using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class UserSession_Loggin
    {
		public int? UserSessionID { get; set; }
		public int? UserID { get; set; }
		public string? Token { get; set; }
		public string? DeviceName { get; set; }
		public string? Ip { get; set; }
		public DateTime? CreateTime { get; set; }
		public int? DeleteStatus { get; set; }
	}
}
