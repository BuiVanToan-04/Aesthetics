using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.Response
{
	public class ResponseBooking
	{
		public int? BookingID { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateTime? BookingCreation { get; set; }
	}

	public class ResponseBookingData : ResponseData
	{
		public List<ResponseBooking>? Data { get; set; }
	}

	public class ResponseBooking_Assignment
	{
		public int AssignmentID { get; set; }
		public int? BookingID { get; set; }
		public int ClinicID { get; set; }
		public string? ClinicName { get; set; }
		public int ProductsOfServicesID { get; set; }
		public string? ProductsOfServicesName { get; set; }
		public string? UserName { get; set; }
		public string? ServiceName { get; set; }
		public int? NumberOrder { get; set; }
		public DateTime AssignedDate { get; set; }
	}

	public class ResponseBooking_AssignmentData : ResponseData
	{
		public List<ResponseBooking_Assignment>? Data { get; set; }
	}

	public class ResponseBooking_Ser_Ass : ResponseData
	{
		public List<Booking_Loggin>? booking_Loggins { get; set; }
		public List<Booking_AssignmentLoggin>? Booking_AssData { get; set; }

		public List<Booking_ServicessLoggin>? Booking_SerData { get; set; }
	}

	public class ResponseBookingUpdate_Ser_Ass : ResponseData
	{
		public List<Booking_AssignmentLoggin>? Booking_AssData_Insert { get; set; }
		public List<Booking_AssignmentLoggin>? Booking_AssData_Update { get; set; }
		public List<Booking_ServicessLoggin>? Booking_SerData_Insert { get; set; }
		public List<Booking_ServicessLoggin>? Booking_SerData_Update { get; set; }
	}
}
