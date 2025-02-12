using Aesthetics.DTO.NetCore.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
    public class SupplierRequest
    {
		public string SupplierName { get; set; }
	}

	public class Update_Supplier
	{
		public int SupplierID { get; set; }
		public string SupplierName { get; set; }
	}

	public class GetList_SearchSupplier
	{
		public int? SupplierID { get; set; }
		public string? SupplierName { get; set; }
	}


	public class Delete_Supplier
	{
		public int SupplierID { get; set; }
	}
}
