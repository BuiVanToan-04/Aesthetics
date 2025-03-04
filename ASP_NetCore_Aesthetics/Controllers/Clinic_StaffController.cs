using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using ASP_NetCore_Aesthetics.Loggin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

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
				//if (responseData.ResponseCode == 1)
				//{
				//	var cacheKey = "GetClinic_Caching";
				//	await _cache.RemoveAsync(cacheKey);
				//}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Insert_ClinicStaff} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpGet("GetList_SearchClinicStaff")]
		public async Task<IActionResult> GetList_SearchClinicStaff(Clinic_StaffGetList getList_)
		{
			var listClinic = new List<ResponesClinic>();
			try
			{
				var responseData = await _clinic_StaffRepository.GetList_Clinic_Staff(getList_);
				return Ok(responseData);
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
