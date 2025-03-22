using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.RequestData;
using ASP_NetCore_Aesthetics.Loggin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ASP_NetCore_Aesthetics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ControllerBase
    {
        private IVouchersRepository _vouchersRepository;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public VouchersController(IVouchersRepository vouchersRepository, IDistributedCache cache, 
			ILoggerManager loggerManager)
		{
			_vouchersRepository = vouchersRepository;
			_cache = cache;
			_loggerManager = loggerManager;
		}

		[HttpPost("Insert_Vouchers")]
		public async Task<IActionResult> Insert_Vouchers(VouchersRequest insert_)
		{
			try
			{
				//1.Insert_Vouchers 
				var responseData = await _vouchersRepository.Insert_Vouchers(insert_);
				//2. Lưu log request
				_loggerManager.LogInfo("Insert_Vouchers Request: " + JsonConvert.SerializeObject(insert_));
				//3. Lưu log data response
				_loggerManager.LogInfo("Insert_Vouchers Response data: " + JsonConvert.SerializeObject(responseData.vouchers_Loggins));
				//if (responseData.ResponseCode == 1)
				//{
				//	var cacheKey = "GetSupplier_Cache";
				//	await _cache.RemoveAsync(cacheKey);
				//}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Insert_Vouchers} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
