using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
	public class User_CreateAccount
	{
		public string UserName { get; set; }
		public string PassWord { get; set; }
		public string? ReferralCode { get; set; }
	}

	public class User_Update
	{
		public int UserID { get; set; }
		public string? Email { get; set; }
		public DateTime? DateBirth { get; set; }
		public string? Sex { get; set; }
		public string? Phone { get; set; }
		public string? Addres { get; set; }
		public string? IDCard { get; set; }
	}
	public class GetList_SearchUser
	{
		public int? UserID { get; set; }
		public string? UserName { get; set; }
	}

	public class User_Delete
	{
		public int UserID { get; set; }
	}
}
