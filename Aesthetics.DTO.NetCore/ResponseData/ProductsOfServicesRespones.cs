using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
	public class ProductsOfServicesRespones
	{
		public int ProductsOfServicesID { get; set; }
		public string ProductsOfServicesName { get; set; }
		public string ProductsOfServicesType { get; set; }
	}

	public class ProductsOfServicesData : ResponseData
	{
		public List<ProductsOfServicesRespones> Data { get; set; }
	}

	public class ProductsOfServices_Loggin : ResponseData
	{
		public List<ProductsOfServices_Logginn>? productOfServicess_Loggin { get; set; }
	}
}
