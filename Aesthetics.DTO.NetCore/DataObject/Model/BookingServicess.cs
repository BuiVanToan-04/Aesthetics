using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
	public class BookingServicess
	{
		public int BookingServiceID { get; set; }
		public int? BookingID { get; set; }
		public int? ServiceID { get; set; }
		public int? ProductsOfServicesID { get; set; }
		public int? DeleteStatus { get; set; }
		public Booking Booking { get; set; }
		public Servicess Servicess { get; set; }
	}
}
