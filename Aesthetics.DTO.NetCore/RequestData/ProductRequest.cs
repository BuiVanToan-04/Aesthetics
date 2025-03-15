using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
    public class ProductRequest
    {
		public int ProductsOfServicesID { get; set; }
		public int SupplierID { get; set; }
		public string? ProductName { get; set; }
		public string? ProductDescription { get; set; }
		public double? SellingPrice { get; set; }
		public int? Quantity { get; set; }
		public string? ProductImages { get; set; }
	}
	public class Update_Products
	{
		public int ProductID { get; set; }
		public string? ProductName { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public int? SupplierID { get; set; }
		public string? ProductDescription { get; set; }
		public double? SellingPrice { get; set; }
		public int? Quantity { get; set; }
		public string? ProductImages { get; set; }
	}
	public class Delete_Products
	{
		public int ProductID { get; set; }
	}

	public class GetList_SearchProducts
	{
		public int? ProductID { get; set; }
		public string? ProductName { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? SupplierName { get; set; }
	}
}
