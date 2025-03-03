using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.Model
{
	public class Booking
	{
		[Key]
		public int BookingID { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateTime? BookingCreation { get; set; }
		public int? DeleteStatus { get; set; }
		public ICollection<BookingServicess> Booking_Servicesses { get; set; }
		public ICollection<BookingAssignment> Booking_Assignment { get; set; }
	}
}
