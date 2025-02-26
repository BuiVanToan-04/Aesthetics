using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
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
using XAct;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClinicController : ControllerBase
	{
		private IClinicRepository _clinicRepository;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public ClinicController(IClinicRepository clinicRepository, IDistributedCache cache,
			ILoggerManager loggerManager)
		{
			_clinicRepository = clinicRepository;
			_cache = cache;
			_loggerManager = loggerManager;
		}
		[HttpPost("Insert_Clinic")]
		public async Task<IActionResult> Insert_Clinic(ClinicRequest clinic)
		{
			try
			{
				//1.Insert Clinic
				var responseData = await _clinicRepository.Insert_Clinic(clinic);
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetClinic_Caching";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Insert Clinic Request: " + JsonConvert.SerializeObject(clinic));
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Insert Clinic} Message: " + ex.Message + 
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("Update_Clinic")]
		public async Task<IActionResult> Update_Clinic(Update_Clinic update_)
		{
			try
			{
				//1.Update clinic
				var responseData = await _clinicRepository.Update_Clinic(update_);
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetClinic_Caching";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Update Clinic Request: " + JsonConvert.SerializeObject(update_));
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update Clinic} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_Clinic")]
		public async Task<IActionResult> Delete_Clinic(Delete_Clinic delete_)
		{
			try
			{
				var responseData = await _clinicRepository.Delete_Clinic(delete_);
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetClinic_Caching";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Delete Clinic Request: " + JsonConvert.SerializeObject(delete_));
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Delete Clinic} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpGet("GetList_SearchClinic")]
		public async Task<IActionResult> GetList_SearchClinic(GetList_Search getList_)
		{
			var listClinic = new List<ResponesClinic>();
			try
			{
				// Khóa để lưu trữ dữ liệu trong Redis
				var cacheKey = "GetClinic_Caching";

				// 1. Lấy dữ liệu từ Redis Cache
				byte[] cachedData = await _cache.GetAsync(cacheKey);
				string cachedDataString = null;
				//1.1 Lưu log request
				_loggerManager.LogInfo("GetList_SearchClinic Request: " + JsonConvert.SerializeObject(getList_));

				// 1.2. Nếu Redis Cache có dữ liệu, giải mã dữ liệu từ cache và trả về client
				if (cachedData != null)
				{
					cachedDataString = Encoding.UTF8.GetString(cachedData);
					listClinic = JsonConvert.DeserializeObject<List<ResponesClinic>>(cachedDataString);

					//1.3 Lọc dữ liệu trong cache khi có request
					listClinic = listClinic
						.Where(c =>
							(getList_.ClinicID == null || c.ClinicID == getList_.ClinicID) &&
							(string.IsNullOrEmpty(getList_.ClinicName) || c.ClinicName.Contains(getList_.ClinicName, StringComparison.OrdinalIgnoreCase)) &&
							(getList_.ProductsOfServicesID == null || c.ProductsOfServicesID == getList_.ProductsOfServicesID) &&
							(string.IsNullOrEmpty(getList_.ProductsOfServicesName) || c.ProductsOfServicesName.Contains(getList_.ProductsOfServicesName, StringComparison.OrdinalIgnoreCase))
						)
						.ToList();
					//1.4 Lưu log dữ liệu clinic trả về trong cache
					_loggerManager.LogInfo("GetList_SearchClinic cache: " + cachedDataString);

					//1.5 Lưu log kết quả dữ liệu khi có request
					_loggerManager.LogInfo("GetList_SearchClinic cache Request: " + JsonConvert.SerializeObject(listClinic));
					return Ok(listClinic);
				}
				else
				{
					// 2. Nếu cache không có dữ liệu, gọi DB
					var responseData = await _clinicRepository.GetList_SearchClinic(getList_);

					//2.1Chuyển đổi dữ liệu lấy từ DB thành danh sách đối tượng ResponesClinic
					listClinic = responseData?.Data?.Select(c => new ResponesClinic
					{
						ClinicID = c.ClinicID,
						ClinicName = c.ClinicName,
						ProductsOfServicesID = c.ProductsOfServicesID,
						ProductsOfServicesName = c.ProductsOfServicesName,
					}).ToList() ?? new List<ResponesClinic>();

					//2.2Lưu danh sách vào Redis Cache
					cachedDataString = JsonConvert.SerializeObject(listClinic);
					var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

					//2.3Cấu hình cache hết hạn
					var options = new DistributedCacheEntryOptions()
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.3 Nếu lấy thành công danh danh thì lưu cache
					if(responseData.ResponseCode == 1)
					{
						await _cache.SetAsync(cacheKey, dataToCache, options);
					}

					//2.4. Lưu log dữ liệu servicess trong db
					_loggerManager.LogInfo("GetList_SearchClinic db: " + cachedDataString);

					return Ok(responseData);
				}
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error GetList_SearchClinic} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
