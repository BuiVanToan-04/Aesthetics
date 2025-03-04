using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class Permission_Loggin
    {
		public int? PermissionID { get; set; }
		public int? UserID { get; set; }
		public string? UserName { get; set; }
		public int? FunctionID { get; set; }
		public int? IsView { get; set; }
		public int? IsInsert { get; set; }
		public int? IsUpdate { get; set; }
		public int? IsDelete { get; set; }
	}
}
