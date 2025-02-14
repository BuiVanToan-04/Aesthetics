using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.RequestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingsController : ControllerBase
	{
		private IBookingsRepository _bookings;
		public BookingsController(IBookingsRepository bookings)
		{
			_bookings = bookings;
		}
		[HttpPost("Insert_Booking")]
		public async Task<IActionResult> Insert_Booking(BookingRequest request)
		{
			try
			{
				var responseData = await _bookings.Insert_Booking(request);
				return Ok(responseData);
			}
			catch (Exception ex) 
			{
				return Ok(ex.StackTrace);
			}
		}
	}
}
