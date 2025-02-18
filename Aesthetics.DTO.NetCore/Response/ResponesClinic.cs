using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
	public class ResponesClinic
	{
		public int ClinicID { get; set; }
		public string ClinicName { get; set; }
		public int ProductsOfServicesID { get; set; }
		public string ProductsOfServicesName { get; set; }
	}

	public class ResponesClinicData : ResponseData
	{
		public List<ResponesClinic> Data { get; set; }
	}
}
