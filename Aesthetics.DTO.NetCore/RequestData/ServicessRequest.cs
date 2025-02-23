using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
	public class ServicessRequest
	{
		public int ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? ServiceName { get; set; }
		public string? Description { get; set; }
		public string? ServiceImage { get; set; }
		public float? PriceService { get; set; }
	}

	public class Update_Servicess
	{
		public int ServiceID { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? ServiceName { get; set; }
		public string? Description { get; set; }
		public string? ServiceImage { get; set; }
		public float? PriceService { get; set; }
	}
	public class Delete_Servicess
	{
		public int ServiceID { get; set; }
	}

	public class GetList_SearchServicess
	{
		public int? ServiceID { get; set; }
		public string? ServiceName { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
	}

	public class ExportExcel
	{
		public string filePath { get; set; }
	}
}
