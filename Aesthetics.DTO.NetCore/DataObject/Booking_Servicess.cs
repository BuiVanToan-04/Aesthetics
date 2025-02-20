using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
	public class Booking_Servicess
	{
		public int BookingServiceID { get; set; }
		public int BookingID { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public int? ServiceID { get; set; }
		public string? ServiceName { get; set; }
		public int? NumberOrder { get; set; }
		public DateTime AssignedDate { get; set; }
		public Booking Booking { get; set; }
		public Servicess Servicess { get; set; }
		public ICollection<Booking_Assignment> Booking_Assignment { get; set; }
	}
}
