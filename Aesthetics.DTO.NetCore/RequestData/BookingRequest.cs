﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.RequestData
{
	public class BookingRequest
	{
		public int ServiceID { get; set; }
		public string UserName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateTime ScheduledDate { get; set; }
	}

	public class Update_Booking
	{
		public int BookingID { get; set; }
		public string? UserName { get; set; }
		public int? ServiceID { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateTime? ScheduledDate { get; set; }
	}

	public class Delete_Booking
	{
		public int BookingID { get; set; }
	}

	public class GetList_SearchBooking
	{
		public int? BookingID { get; set; }
		public string? UserName { get; set; }
		public DateTime? ScheduledDate { get; set; }
	}
}
