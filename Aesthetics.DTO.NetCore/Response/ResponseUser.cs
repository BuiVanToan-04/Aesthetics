using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
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

	public class ResponseUserData : ResponseData
	{
		public List<ResponseUser> Data { get; set; }
	}
}
