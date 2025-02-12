using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
	public class TypeProductsOfServicesRequest
	{
		public string ProductsOfServicesName { get; set; }
		public string ProductsOfServicesType { get; set; }
	}

	public class Update_TypeProductsOfServices
	{
		public int ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? ProductsOfServicesType { get; set; }
	}

	public class Delete_TypeProductsOfServices
	{
		public int ProductsOfServicesID { get; set; }
	}

	public class GetList_SearchTypeProductsOfServices
	{
		public int? ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? ProductsOfServicesType { get; set; }
	}
}
