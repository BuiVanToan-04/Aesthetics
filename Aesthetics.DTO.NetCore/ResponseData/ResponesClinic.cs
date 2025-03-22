using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
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
		public string ClinicStatus { get; set; }
	}

	public class ResponesClinic_Loggin : ResponseData
	{
		public List<ResponesClinic> Data { get; set; }
		public List<Clinic_Loggin>? listClinics { get; set; }
	}

	public class ResponesClinic_DeleteLoggin : ResponseData
	{
		public List<Clinic_Loggin>? clinic_Loggins { get; set; }
		public List<Booking_AssignmentLoggin>? booking_AssignmentLoggins { get; set; }
		public List<Clinic_Staff_Loggin>? clinic_Staff_Loggins { get; set; }
	}
}
