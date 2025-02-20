using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject
{
	public class Booking
	{
		[Key]
		public int BookingID { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateTime? BookingCreation { get; set; }
		public DateTime ScheduledDate { get; set; }
		public int? DeleteStatus { get; set; }
		public ICollection<Booking_Servicess> Booking_Servicesses { get; set; }
	}
}
