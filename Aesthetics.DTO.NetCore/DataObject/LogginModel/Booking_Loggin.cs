using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DTO.NetCore.DataObject.LogginModel
{
    public class Booking_Loggin
    {
		public int BookingID { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateTime? BookingCreation { get; set; }
		public int? DeleteStatus { get; set; }
	}
}
