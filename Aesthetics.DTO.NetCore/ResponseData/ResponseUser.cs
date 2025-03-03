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
	public class ResponseUser
	{
		public int UserID { get; set; }
		public string UserName { get; set; }
		public string? Email { get; set; }
		public DateTime? DateBirth { get; set; }
		public string? Sex { get; set; }
		public string? Phone { get; set; }
		public string? Addres { get; set; }
		public string? IDCard { get; set; }
		public string? TypePerson { get; set; }
	}

	public class ResponseUser_InsertLoggin: ResponseData
	{
		public List<User_Loggin>? listUser { get; set; }
		public List<Carts_Loggin>? listCarts { get; set; }
	}

	public class ResponseUser_UpdateLoggin : ResponseData
	{
		public List<User_Loggin>? listUser { get; set; }
	}

	public class ResponseUser_DeleteLoggin: ResponseData
	{
		public List<User_Loggin>? listUser { get; set; }
		public List<Carts_Loggin>? listCarts { get; set; }
		public List<Clinic_Staff_Loggin>? listClinicStaff { get; set; }
		public List<Wallets_Loggin>? listWallets { get; set; }
		public List<Permission_Loggin>? listPermission { get; set; }
		public List<UserSession_Loggin>? listUserSession { get; set; }
	}

	public class ResponseUserData : ResponseData
	{
		public List<ResponseUser> Data { get; set; }
	}
}
