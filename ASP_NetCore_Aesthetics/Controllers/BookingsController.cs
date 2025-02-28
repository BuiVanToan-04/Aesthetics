using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using ASP_NetCore_Aesthetics.Loggin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingsController : ControllerBase
	{
		private IBookingsRepository _bookingRepository;
		private readonly ILoggerManager _loggerManager;
		public BookingsController(IBookingsRepository bookingRepository, ILoggerManager loggerManager)
		{
			_bookingRepository = bookingRepository;
			_loggerManager = loggerManager;
		}
		[HttpPost("Insert_Booking")]
		public async Task<IActionResult> Insert_Booking(BookingRequest request)
		{
			try
			{
				//1. Insert_Booking
				var responseData = await _bookingRepository.Insert_Booking(request);
				//2. Lưu log Insert_Booking Request 
				_loggerManager.LogInfo("Insert_Booking Request: " + JsonConvert.SerializeObject(request));

				//3. Lưu log Insert_Booking_Assignment Request 
				_loggerManager.LogInfo("Insert_Booking_Assignment Request: " + JsonConvert.SerializeObject(responseData.Booking_AssData));

				//4. Lưu log Insert_Booking_Servicess Request 
				_loggerManager.LogInfo("Insert_Booking_Servicess Request: " + JsonConvert.SerializeObject(responseData.Booking_SerData));
				return Ok(responseData);
			}
			catch (Exception ex) 
			{
				_loggerManager.LogError("{Error Insert_Booking} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("Update_Booking")]
		public async Task<IActionResult> Update_Booking(Update_Booking request)
		{
			try
			{
				//1. Update_Booking
				var responseData = await _bookingRepository.Update_Booking(request);

				//2. Lưu log Insert_Booking Request 
				_loggerManager.LogInfo("Update_Booking Request: " + JsonConvert.SerializeObject(request));

				//3. Lưu log Update_Booking_Assignment Request 
				_loggerManager.LogInfo("Update_Booking_Assignment Request: " + JsonConvert.SerializeObject(responseData.Booking_AssData_Update));

				//4. Lưu log Insert_Booking_Assignment Request 
				_loggerManager.LogInfo("Insert_Booking_Assignment Request: " + JsonConvert.SerializeObject(responseData.Booking_AssData_Insert));

				//5. Lưu log Update_Booking_Servicess Request 
				_loggerManager.LogInfo("Update_Booking_Servicess Request: " + JsonConvert.SerializeObject(responseData.Booking_SerData_Update));

				//6. Lưu log Insert_Booking_Servicess Request 
				_loggerManager.LogInfo("Insert_Booking_Servicess Request: " + JsonConvert.SerializeObject(responseData.Booking_SerData_Insert));
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update_Booking} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_Booking")]
		public async Task<IActionResult> Delete_Booking(Delete_Booking delete_)
		{
			try
			{
				//1. Delete_Booking
				var responseData = await _bookingRepository.Delete_Booking(delete_);

				//2. Lưu log Insert_Booking Request 
				_loggerManager.LogInfo("Delete_Booking Request: " + JsonConvert.SerializeObject(delete_));

				//3. Lưu log Delete_Booking_Assignment Request 
				_loggerManager.LogInfo("Delete_Booking_Assignment Request: " + JsonConvert.SerializeObject(responseData.Booking_AssData));

				//4. Lưu log Delete_Booking_Servicess Request 
				_loggerManager.LogInfo("Delete_Booking_Servicess Request: " + JsonConvert.SerializeObject(responseData.Booking_SerData));

				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update_Booking} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
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
				return Ok(ex.Message);
			}
		}

		[HttpGet("GetList_SearchBooking_Assignment")]
		public async Task<IActionResult> GetList_SearchBooking_Assignment(GetList_SearchBooking_Assignment getList_)
		{
			try
			{
				var responseData = await _bookingRepository.GetList_SearchBooking_Assignment(getList_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.Message);
			}
		}
	}
}
