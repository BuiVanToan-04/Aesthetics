using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using ASP_NetCore_Aesthetics.Loggin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServicessController : ControllerBase
	{
		private IServicessRepository _servicess;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public ServicessController(IServicessRepository servicess, IDistributedCache cache, 
			ILoggerManager loggerManager)
		{
			_servicess = servicess;
			_cache = cache;
			_loggerManager = loggerManager;
		}

		[HttpPost("Insert_Servicess")]
		public async Task<IActionResult> Insert_Servicess(ServicessRequest servicess)
		{
			try
			{
				//1. Insert Servicess
				var responseData = await _servicess.Insert_Servicess(servicess);
				//2. Lưu log request
				_loggerManager.LogInfo("Insert Servicess Request: " + JsonConvert.SerializeObject(servicess));
				//3. Lưu log data servicess
				_loggerManager.LogInfo("Insert Servicess Response data : " + JsonConvert.SerializeObject(responseData.servicess_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetServicess_Caching";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex) 
			{
				_loggerManager.LogError("{Error Insert Servicess} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
		[HttpPost("Update_Servicess")]
		public async Task<IActionResult> Update_Servicess(Update_Servicess servicess)
		{
			try
			{
				//1.Update Servicess
				var responseData = await _servicess.Update_Servicess(servicess);
				//2. Lưu log request
				_loggerManager.LogInfo("Update Servicess Request: " + JsonConvert.SerializeObject(servicess));
				//3. Lưu log data servicess
				_loggerManager.LogInfo("Update Servicess Response data : " + JsonConvert.SerializeObject(responseData.servicess_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetServicess_Caching";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update Servicess} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("ExportServicessToExcel")]
		public async Task<IActionResult> ExportServicessToExcel(ExportExcel filePath)
		{
			try
			{
				//1.Export excel
				var responseData = await _servicess.ExportServicessToExcel(filePath);
				//2. Lưu log
				_loggerManager.LogInfo("ExportServicessToExcel Servicess: " + JsonConvert.SerializeObject(filePath));
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error ExportServicessToExcel} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_Servicess")]
		public async Task<IActionResult> Delete_Servicess(Delete_Servicess servicess)
		{
			try
			{
				//1.Delete servicess
				var responseData = await _servicess.Delete_Servicess(servicess);
				//2. Lưu log request
				_loggerManager.LogInfo("Delete Servicess Request: " + JsonConvert.SerializeObject(servicess));
				//3. Lưu log data delete Servicess
				_loggerManager.LogInfo("Delete Servicess Response Data: " + JsonConvert.SerializeObject(responseData.servicess_Loggins));
				//4. Lưu log data delete booking_Servicess
				_loggerManager.LogInfo("Delete Booking_Servicess Response Data: " + JsonConvert.SerializeObject(responseData.booking_ServicessLoggins));
				//5. Lưu log data delete Comment
				_loggerManager.LogInfo("Delete Comment Response Data: " + JsonConvert.SerializeObject(responseData.comment_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetServicess_Caching";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.Message);
			}
		}
		[HttpGet("GetList_SearchServicess")]
		public async Task<IActionResult> GetList_SearchServicess(GetList_SearchServicess servicess)
		{
			try
			{
				var listServicess = new List<ResponseServicess>();
				// Khóa để lưu trữ dữ liệu trong Redis
				var cacheKey = "GetServicess_Caching";

				// 1. Lấy dữ liệu từ Redis Cache
				byte[] cachedData = await _cache.GetAsync(cacheKey);

				//1.1 Lưu log request
				_loggerManager.LogInfo("GetList_SearchServicess Requets: " + JsonConvert.SerializeObject(servicess));

				// 1.2. Nếu Redis Cache có dữ liệu, giải mã dữ liệu từ cache và trả về client
				if (cachedData != null)
				{
					var cachedDataString = Encoding.UTF8.GetString(cachedData);
					listServicess = JsonConvert.DeserializeObject<List<ResponseServicess>>(cachedDataString);

					//1.3. Lọc dữ liệu khi có request 
					listServicess = listServicess
						.Where(s => 
						(servicess.ServiceID == null || s.ServiceID == servicess.ServiceID) &&
						(string.IsNullOrEmpty(servicess.ServiceName) || s.ServiceName.Contains(servicess.ServiceName, StringComparison.OrdinalIgnoreCase)) &&
						(servicess.ProductsOfServicesID == null || s.ProductsOfServicesID == servicess.ProductsOfServicesID)
						).ToList();
					//1.4. Lưu log dữ liệu servicess trả về trong cache
					_loggerManager.LogInfo("GetList_SearchServicess cache: " + cachedDataString);

					//1.5 Lưu log kết quả dữ liệu khi có request
					_loggerManager.LogInfo("GetList_SearchServicess cache Request: " + JsonConvert.SerializeObject(listServicess));
					return Ok(listServicess);
				}
				else
				{
					//2. Nếu Redis Cache không có dữ liệu, gọi vào database để lấy dữ liệu mới
					var responseData = await _servicess.GetList_SearchServicess(servicess);

					//2.1. Chuyển đổi dữ liệu lấy từ DB thành danh sách đối tượng ResponseServicess
					listServicess = responseData?.Data?.Select(s => new ResponseServicess
					{
						ServiceID = s.ServiceID,
						ServiceName = s.ServiceName,
						ProductsOfServicesID = s.ProductsOfServicesID,
						Description = s.Description,
						ServiceImage = s.ServiceImage,
						PriceService = s.PriceService,
					}).ToList() ?? new List<ResponseServicess>();

					//2.2. Chuyển danh sách dịch vụ thành chuỗi JSON để lưu vào cache
					var cachedDataString = JsonConvert.SerializeObject(listServicess);
					var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

					//2.3 Cấu hình thời gian hết hạn cho Redis Cache
					DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.4. Lưu log dữ liệu servicess trong db
					_loggerManager.LogInfo("GetList_SearchServicess db: " + cachedDataString);

					//2.5. Kiểm tra nếu lấy thành công danh sách thì lưu cache
					if(responseData.ResponseCode == 1)
					{
						await _cache.SetAsync(cacheKey, dataToCache, options);
					}
					return Ok(responseData);
				}
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error GetList_SearchServicess} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
