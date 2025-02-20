using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
	public class Clinic
	{
		[Key]
		public int ClinicID { get; set; }
		public string ClinicName { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public string ProductsOfServicesName { get; set; }
		public int ClinicStatus { get; set; }
		public ICollection<Clinic_Staff> Clinic_Staff { get; set; }
	}
}
