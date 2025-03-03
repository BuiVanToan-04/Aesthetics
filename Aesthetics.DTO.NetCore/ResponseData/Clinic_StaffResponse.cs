using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
    public class Clinic_StaffResponse
    {
		public int ClinicStaffID { get; set; }
		public int ClinicID { get; set; }
		public int UserID { get; set; }
	}

	public class Clinic_StaffData : ResponseData
	{
		public List<Clinic_StaffResponse> Data { get; set; }
	}
}
