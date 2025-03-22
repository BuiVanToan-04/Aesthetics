using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
	public class Clinic_Staff_Loggin
	{
		public int ClinicStaffID { get; set; }
		public int? ClinicID { get; set; }
		public string? ClinicName { get; set; }
		public int? UserID { get; set; }
		public string? UserName { get; set; }
		public string? Phone { get; set; }
		public string? TypePerson { get; set; }
	}
}
