using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
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

	public class ResponseSupplierData : ResponseData
	{
		public List<ResponseSupplier> Data { get; set; }
	}
}
