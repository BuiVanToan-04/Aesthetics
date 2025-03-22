using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
	public class ResponseServicess
	{
		public int? ServiceID { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? ServiceName { get; set; }
		public string? Description { get; set; }
		public string? ServiceImage { get; set; }
		public double? PriceService { get; set; }
	}

	public class ResponseServicess_Loggin : ResponseData
	{
		public List<ResponseServicess> Data { get; set; }
		public List<Servicess_Loggin>? servicess_Loggins { get; set; }
	}


	public class ResponseServicess_LogginDelete : ResponseData
	{
		public List<Servicess_Loggin>? servicess_Loggins { get; set; }
		public List<Booking_ServicessLoggin>? booking_ServicessLoggins { get; set; }
		public List<Comment_Loggin>? comment_Loggins { get; set; }
	}
}
