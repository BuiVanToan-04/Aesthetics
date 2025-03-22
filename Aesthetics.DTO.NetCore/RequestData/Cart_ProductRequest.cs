using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
    public class Cart_ProductRequest
    {
		public int CartID { get; set; }
		public int ProductID { get; set; }
		public int Quantity { get; set; }
	}

	public class Update_Cart_ProductRequest
	{
		public int CartProductID { get; set; }
		public int Quantity { get; set; }
	}

	public class Delete_Cart_ProductRequest
	{
		public int CartProductID { get; set; }
	}

	public class GetList_SearchCart_ProductRequest
	{
		public int CartID { get; set; }
	}
}
