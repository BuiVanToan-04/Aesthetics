using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
    public class ResponseProducts
    {
		public int? ProductID { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public int? SupplierID { get; set; }
		public string? ProductName { get; set; }
		public string? ProductDescription { get; set; }
		public double? SellingPrice { get; set; }
		public int? Quantity { get; set; }
		public string? ProductImages { get; set; }
		public int? DeleteStatus { get; set; }
	}

	public class ResponseGetListProducts
	{
		public int? ProductID { get; set; }
		public string? ProductName { get; set; }
		public string? ProductDescription { get; set; }
		public double? SellingPrice { get; set; }
		public int? Quantity { get; set; }
		public string? ProductImages { get; set; }
		public int? DeleteStatus { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? SupplierName { get; set; }
	}

	public class ResponseProducts_Loggin : ResponseData
	{
		public List<Products_Loggin>? products_Loggins { get; set; }
	}

	public class ResponseProducts_LogginGetList : ResponseData
	{
		public List<ResponseGetListProducts>? Data { get; set; }
	}

	public class ResponseProducts_LogginDelete : ResponseData
	{
		public List<Products_Loggin>? products_Loggins { get; set; }
		public List<Comment_Loggin>? comment_Loggins { get; set; }
		public List<CartProducts_Loggin>? cartProducts_Loggins { get; set; }
	}
}
