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
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetServicess_Caching";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Insert Servicess: " + JsonConvert.SerializeObject(servicess));
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
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetServicess_Caching";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Update Servicess: " + JsonConvert.SerializeObject(servicess));
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
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetServicess_Caching";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Delete Servicess: " + JsonConvert.SerializeObject(servicess));
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
			var listServicess = new List<ResponseServicess>();
			try
			{
				// Khóa để lưu trữ dữ liệu trong Redis
				var cacheKey = "GetServicess_Caching";

				// 1. Lấy dữ liệu từ Redis Cache
				byte[] cachedData = await _cache.GetAsync(cacheKey);

				//1.1 Lưu log request
				_loggerManager.LogInfo("GetList_SearchServicess: " + JsonConvert.SerializeObject(servicess));

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
					_loggerManager.LogInfo("GetList_SearchServicess cache: " + JsonConvert.SerializeObject(cachedDataString));
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

					//2.3Cấu hình thời gian hết hạn cho Redis Cache
					DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
						//-- Cache sẽ tồn tại tối đa 5 phút
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						//-- Nếu không có truy cập trong 3 phút, cache sẽ bị xóa
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.4. Lưu log dữ liệu servicess trong db
					_loggerManager.LogInfo("GetList_SearchServicess db: " + JsonConvert.SerializeObject(cachedDataString));

					//2.5. Lưu dữ liệu vào Redis Cache
					await _cache.SetAsync(cacheKey, dataToCache, options);
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
