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
		private IBookingsRepository _bookingRepository;
		public BookingsController(IBookingsRepository bookingRepository)
		{
			_bookingRepository = bookingRepository;
		}
		[HttpPost("Insert_Booking")]
		public async Task<IActionResult> Insert_Booking(BookingRequest request)
		{
			try
			{
				var responseData = await _bookingRepository.Insert_Booking(request);
				return Ok(responseData);
			}
			catch (Exception ex) 
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpPost("Update_Booking")]
		public async Task<IActionResult> Update_Booking(Update_Booking request)
		{
			try
			{
				var responseData = await _bookingRepository.Update_Booking(request);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpDelete("Delete_Booking")]
		public async Task<IActionResult> Delete_Booking(Delete_Booking delete_)
		{
			try
			{
				var responseData = await _bookingRepository.Delete_Booking(delete_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpGet("GetList_SearchBooking")]
		public async Task<IActionResult> GetList_SearchBooking(GetList_SearchBooking getList_)
		{
			try
			{
				var responseData = await _bookingRepository.GetList_SearchBooking(getList_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}
	}
}
