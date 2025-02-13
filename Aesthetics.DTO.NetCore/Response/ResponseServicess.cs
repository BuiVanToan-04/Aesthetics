using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
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
	public class ResponseServicessData : ResponseData
	{
		public List<ResponseServicess> Data { get; set; }
	}
}
