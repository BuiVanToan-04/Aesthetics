using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
	public class ClinicRequest
	{
		public string ClinicName { get; set; }
		public int ProductsOfServicesID { get; set; }
		public string ProductsOfServicesName { get; set; }
	}

	public class Update_Clinic
	{
		public int ClinicID { get; set; }
		public string? ClinicName { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
	}

	public class Delete_Clinic
	{
		public int ClinicID { get; set; }
	}

	public class GetList_Search
	{
		public int? ClinicID { get; set; }
		public string? ClinicName { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
	}
}
