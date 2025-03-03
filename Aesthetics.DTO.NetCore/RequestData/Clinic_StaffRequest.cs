using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
    public class Clinic_StaffRequest
    {
		public int ClinicID { get; set; }
		public int UserID { get; set; }
	}

	public class Clinic_StaffUpdate
	{
		public int ClinicStaffID { get; set; }
		public int? ClinicID { get; set; }
		public int? UserID { get; set; }
	}

	public class Clinic_StaffDelete 
	{
		public int ClinicStaffID { get; set; }
	}

	public class Clinic_StaffGetList
	{
		public int? ClinicStaffID { get; set; }
		public int? ClinicID { get; set; }
		public int? UserID { get; set; }
	}
}
