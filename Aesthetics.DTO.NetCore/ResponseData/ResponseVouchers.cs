using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.ResponseVouchers
{
	public class ResponseVouchers_Loggin : ResponseData
	{
		public List<Vouchers_Loggin>? Data { get; set; }
		public List<Vouchers_Loggin>? vouchers_Loggins { get; set; }
	}
}
