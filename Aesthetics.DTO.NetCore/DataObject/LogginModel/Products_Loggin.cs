using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class Products_Loggin
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

	public class Products_LogginGetList
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
}
