using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
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
		public int? ClinicID { get; set; }
		public string? ClinicName { get; set; }
		public int? UserID { get; set; }
		public string? UserName { get; set; }
		public string? Phone { get; set; }
		public string? TypePerson { get; set; }
	}

	public class Response_ClinicStaff_Loggin : ResponseData
	{
		public List<Clinic_StaffResponse> Data { get; set; }
		public List<Clinic_Staff_Loggin>? clinic_Staff_Loggins { get; set; }
	}
}
