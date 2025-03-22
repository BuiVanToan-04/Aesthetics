using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.ResponseCart_Product
{
	public class ResponseCart_Product
	{
		public int? CartProductID { get; set; }
		public int? ProductID { get; set; }
		public string? ProductName { get; set; }
		public int? Quantity { get; set; }
		public DateTime? CreateDay { get; set; }
	}

	public class ResponseGetList_SearchCart_Product_Loggin : ResponseData
	{
		public List<ResponseCart_Product>? Data { get; set; }
		public List<CartProducts_Loggin>? cartProduct_Loggins { get; set; }
	}

	public class ResponseCart_Product_Loggin : ResponseData
	{
		public List<CartProducts_Loggin>? cartProduct_Loggins { get; set; }
	}
}
