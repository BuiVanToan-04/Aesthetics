using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
	public class BookingRequest
	{
		public List<int> ServiceIDs { get; set; }
		public int UserID { get; set; }
		public DateTime ScheduledDate { get; set; }
	}

	public class Update_Booking
	{
		public List<int>? ServiceIDs { get; set; }
		public int BookingID { get; set; }
		public int UserID { get; set; }
		public DateTime ScheduledDate { get; set; }
	}

	public class Delete_Booking
	{
		public int BookingID { get; set; }
	}

	public class Insert_Booking_Services
	{
		public int BookingID { get; set; }
		public int ServiceID { get; set; }
	}

	public class Delete_Booking_Services
	{
		public int? BookingServiceID { get; set; }
	}

	public class GetList_SearchBooking
	{
		public int? BookingID { get; set; }
		public int? UserID { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class GetList_SearchBooking_Assignment
	{
		public int? AssignmentID { get; set; }
		public int? BookingID { get; set; }
		public int? ClinicID { get; set; }
		public string? ServiceName { get; set; }
		public DateTime? AssignedDate { get; set; }
	}

	public class GetList_SearchBooking_Services
	{
		public int? BookingID { get; set; }
	}
}
