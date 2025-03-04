using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using ASP_NetCore_Aesthetics.Filter;
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
    public class SupplierController : ControllerBase
    {
        private ISupplierRepository _supplierRepository;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public SupplierController(ISupplierRepository supplierRepository, IDistributedCache cache
			, ILoggerManager loggerManager)
        {
            _supplierRepository = supplierRepository;
			_cache = cache;
			_loggerManager = loggerManager;
        }

        [HttpPost("Insert_Supplier")]
        public async Task<IActionResult> Insert_Supplier(SupplierRequest supplier)
        {
            try
            {
				//1.Insert_Supplier 
				var responseData = await _supplierRepository.Insert_Supplier(supplier);
				//2. Lưu log request
				_loggerManager.LogInfo("Insert_Supplier Request: " + JsonConvert.SerializeObject(supplier));
				//3. Lưu log data response
				_loggerManager.LogInfo("Insert_Supplier Response data: " + JsonConvert.SerializeObject(responseData.supplier_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetSupplier_Cache";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
            }
            catch (Exception ex) 
            {
				_loggerManager.LogError("{Error Insert_Supplier} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
            }
        }

		[HttpPost("Update_Supplier")]
		public async Task<IActionResult> Update_Supplier(Update_Supplier supplier)
		{
			try
			{
				//1.Update_Supplier
				var responseData = await _supplierRepository.Update_Supplier(supplier);
				//2. Lưu log request
				_loggerManager.LogInfo("Update_Supplier Request: " + JsonConvert.SerializeObject(supplier));
				//3. Lưu log data response
				_loggerManager.LogInfo("Update_Supplier Response data: " + JsonConvert.SerializeObject(responseData.supplier_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetSupplier_Cache";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update_Supplier} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_Supplier")]
		public async Task<IActionResult> Delete_Supplier(Delete_Supplier supplier)
		{
			try
			{
				//1. Delete_Supplier
				var responseData = await _supplierRepository.Delete_Supplier(supplier);
				//2. Lưu log request
				_loggerManager.LogInfo("Delete_Supplier Request: " + JsonConvert.SerializeObject(supplier));
				//3. Lưu log data response
				_loggerManager.LogInfo("Update_Supplier Response data: " + JsonConvert.SerializeObject(responseData.supplier_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetSupplier_Cache";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Delete_Supplier} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		//[Filter_Authorization("Update_User", "UPDATE")]
		[HttpGet("GetList_SearchSupplier")]
		public async Task<IActionResult> GetList_SearchSupplier(GetList_SearchSupplier supplier)
		{
			try
			{
				var listSupplier = new List<ResponseSupplier>();
				//Khóa để lưu dữ liệu trong Redis caching
				var cacheKey = "GetSupplier_Cache";

				//1. Lấy dữ liệu trong Redis caching
				byte[] cachedData = await _cache.GetAsync(cacheKey);

				//1.1 Lưu log request
				_loggerManager.LogInfo("GetList_SearchSupplier Request: " + JsonConvert.SerializeObject(supplier));

				//1.2 Nếu Cache có dữ liệu, giải mã trả về Client
				if (cachedData != null)
				{
					var cachedDataString = Encoding.UTF8.GetString(cachedData);
					listSupplier = JsonConvert.DeserializeObject<List<ResponseSupplier>>(cachedDataString);

					//1.3 Lọc dữ liệu khi có request 
					listSupplier = listSupplier
						.Where(s =>
						(supplier.SupplierID == null || s.SupplierID == supplier.SupplierID) &&
						(string.IsNullOrEmpty(supplier.SupplierName) || s.SupplierName.Contains(supplier.SupplierName, StringComparison.OrdinalIgnoreCase))
						).ToList();

					//1.4 Lưu log dữ liệu Supplier trả về trong cache
					_loggerManager.LogInfo("GetList_SearchSupplier cache: " + cachedDataString);

					//1.5 Lưu log kết quả dữ liệu khi có request
					_loggerManager.LogInfo("GetList_SearchSupplier cache Request: " + JsonConvert.SerializeObject(listSupplier));

					return Ok(listSupplier);
				}
				else
				{
					//2. Nếu cache không có dữ liệu gọi vào db
					var responseData = await _supplierRepository.GetList_SearchSupplier(supplier);

					//2.1 Chuyển đổi dữ liệu lấy từ DB thành danh sách đối tượng ResponseServicess
					listSupplier = responseData?.Data?.Select(s => new ResponseSupplier
					{
						SupplierID = s.SupplierID,
						SupplierName = s.SupplierName,
					}).ToList() ?? new List<ResponseSupplier>();

					//2.2. Chuyển danh sách dịch vụ thành chuỗi JSON để lưu vào cache
					var cachedDataString = JsonConvert.SerializeObject(listSupplier);
					var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

					//2.3 Cấu hình thời gian hết hạn cho Redis Cache
					DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.4. Lưu log dữ liệu Supplier trong db
					_loggerManager.LogInfo("GetList_SearchSupplier db: " + cachedDataString);

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
				_loggerManager.LogError("{Error GetList_SearchSupplier} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
