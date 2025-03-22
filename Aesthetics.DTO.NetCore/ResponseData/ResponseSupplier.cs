using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
	public class ResponseSupplier
	{
		public int SupplierID { get; set; }
		public string SupplierName { get; set; }
	}
	public class ResponseSupplier_Loggin : ResponseData
	{
		public List<ResponseSupplier>? Data { get; set; }
		public List<Supplier_Loggin>? supplier_Loggins { get; set; }
	}
}
