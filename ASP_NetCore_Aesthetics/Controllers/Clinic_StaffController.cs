using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using ASP_NetCore_Aesthetics.Loggin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class Clinic_StaffController : ControllerBase
	{
		private IClinic_StaffRepository _clinic_StaffRepository;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public Clinic_StaffController(IClinic_StaffRepository clinic_StaffRepository, IDistributedCache cache,
			ILoggerManager loggerManager)
		{
			_clinic_StaffRepository = clinic_StaffRepository;
			_cache = cache;
			_loggerManager = loggerManager;
		}
		[HttpPost("Insert_ClinicStaff")]
		public async Task<IActionResult> Insert_ClinicStaff(Clinic_StaffRequest clinic_Staff)
		{
			try
			{
				//1.Insert_ClinicStaff
				var responseData = await _clinic_StaffRepository.Insert_Clinic_Staff(clinic_Staff);
				//2. Lưu log request
				_loggerManager.LogInfo("Insert_ClinicStaff Request: " + JsonConvert.SerializeObject(clinic_Staff));
				//3. Lưu log data
				_loggerManager.LogInfo("Insert_ClinicStaff data: " + JsonConvert.SerializeObject(responseData.clinic_Staff_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetClinic_Staff_Cache";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Insert_ClinicStaff} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("Update_ClinicStaff")]
		public async Task<IActionResult> Update_ClinicStaff(Clinic_StaffUpdate clinic_Staff)
		{
			try
			{
				//1.Update_ClinicStaff
				var responseData = await _clinic_StaffRepository.Update_Clinic_Staff(clinic_Staff);
				//2. Lưu log request
				_loggerManager.LogInfo("Update_ClinicStaff Request: " + JsonConvert.SerializeObject(clinic_Staff));
				//3. Lưu log data
				_loggerManager.LogInfo("Update_ClinicStaff data: " + JsonConvert.SerializeObject(responseData.clinic_Staff_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetClinic_Staff_Cache";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update_ClinicStaff} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_ClinicStaff")]
		public async Task<IActionResult> Delete_ClinicStaff(Clinic_StaffDelete clinic_Staff)
		{
			try
			{
				//1.Insert_ClinicStaff
				var responseData = await _clinic_StaffRepository.Delete_Clinic_Staff(clinic_Staff);
				//2. Lưu log request
				_loggerManager.LogInfo("Delete_ClinicStaff Request: " + JsonConvert.SerializeObject(clinic_Staff));
				//3. Lưu log data
				_loggerManager.LogInfo("Delete_ClinicStaff data: " + JsonConvert.SerializeObject(responseData.clinic_Staff_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetClinic_Staff_Cache";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Delete_ClinicStaff} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpGet("GetList_SearchClinicStaff")]
		public async Task<IActionResult> GetList_SearchClinicStaff(Clinic_StaffGetList getList_)
		{
			var listClinic = new List<Clinic_StaffResponse>();

			try
			{
				//Khóa để lưu dữ liệu trong Redis caching
				var cacheKey = "GetClinic_Staff_Cache";

				//1. Lấy dữ liệu trong Redis caching
				byte[] cachedData = await _cache.GetAsync(cacheKey);

				//1.1 Lưu log request
				_loggerManager.LogInfo("GetList_SearchClinic_Staff Request: " + JsonConvert.SerializeObject(getList_));

				//1.2 Nếu Cache có dữ liệu, giải mã trả về Client
				if (cachedData != null)
				{
					var cachedDataString = Encoding.UTF8.GetString(cachedData);
					var cachedResponse = JsonConvert.DeserializeObject<Response_ClinicStaff_Loggin>(cachedDataString);
					listClinic = cachedResponse?.Data ?? new List<Clinic_StaffResponse>();

					//1.3 Lọc dữ liệu khi có request 
					listClinic = listClinic
						.Where(s =>
						(getList_.ClinicStaffID == null || s.ClinicStaffID == getList_.ClinicStaffID) &&
						(getList_.ClinicID == null || s.ClinicID == getList_.ClinicID) &&
						(getList_.UserID == null || s.UserID == getList_.UserID)
						).ToList();

					//1.4 Lưu log dữ liệu Clinic trả về trong cache
					_loggerManager.LogInfo("GetList_SearchClinic_Staff cache: " + cachedDataString);

					//1.5 Lưu log kết quả dữ liệu khi có Clinic
					_loggerManager.LogInfo("GetList_SearchClinic_Staff cache Request: " + JsonConvert.SerializeObject(listClinic));

					return Ok(listClinic);
				}

				else
				{
					//2. Nếu cache không có dữ liệu gọi vào db
					var responseData = await _clinic_StaffRepository.GetList_Clinic_Staff(getList_);

					//2.1 Chuyển đổi dữ liệu lấy từ DB thành danh sách đối tượng ResponseServicess
					listClinic = responseData?.Data?.Select(s => new Clinic_StaffResponse
					{
						ClinicStaffID = s.ClinicStaffID,
						ClinicID = s.ClinicID,
						UserID = s.UserID,
						UserName = s.UserName,
						ClinicName = s.ClinicName,
						Phone = s.Phone,
						TypePerson = s.TypePerson,
					}).ToList() ?? new List<Clinic_StaffResponse>();

					//2.2. Chuyển danh sách dịch vụ thành chuỗi JSON để lưu vào cache
					var cachedDataString = JsonConvert.SerializeObject(responseData);
					var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

					//2.3 Cấu hình thời gian hết hạn cho Redis Cache
					DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.4. Lưu log dữ liệu Clinic trong db
					_loggerManager.LogInfo("GetList_SearchClinic_Staff db: " + cachedDataString);

					//2.5. Kiểm tra nếu lấy thành công danh sách thì lưu cache
					if (responseData.ResponseCode == 1)
					{
						await _cache.SetAsync(cacheKey, dataToCache, options);
					}
					return Ok(responseData);
				}
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error GetList_SearchClinicStaff} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	} 
}
