using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class Clinic_Loggin
    {
		public int? ClinicID { get; set; }
		public string? ClinicName { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public int? ClinicStatus { get; set; }
	}
}
