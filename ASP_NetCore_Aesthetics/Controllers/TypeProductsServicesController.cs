using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.DataObject;
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
	public class TypeProductsServicesController : ControllerBase
	{
		private ITypeProductsOfServicesRepository _repository;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public TypeProductsServicesController(ITypeProductsOfServicesRepository repository, IDistributedCache cache
			, ILoggerManager loggerManager)
		{
			_repository = repository;
			_loggerManager = loggerManager;
			_cache = cache;
		}

		[HttpPost("Insert_TypeProductsOfServices")]
		public async Task<IActionResult> Insert_TypeProductsOfServices(TypeProductsOfServicesRequest request)
		{
			try
			{
				//1. Insert_ProductsOfServices
				var responesData = await _repository.Insert_TypeProductsOfServices(request);
				if (responesData.ResponseCode == 1)
				{
					var cacheKey = "GetProductOfServicess_Cache";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Insert_ProductsOfServices Request: " + JsonConvert.SerializeObject(request));
				}
				return Ok(responesData);
			}
			catch (Exception ex) 
			{
				_loggerManager.LogError("{Error Insert_ProductsOfServices} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("Update_TypeProductsOfServices")]
		public async Task<IActionResult> Update_TypeProductsOfServices(Update_TypeProductsOfServices request)
		{
			try
			{
				//1. Update_ProductsOfServices
				var responesData = await _repository.Update_TypeProductsOfServices(request);
				if (responesData.ResponseCode == 1)
				{
					var cacheKey = "GetProductOfServicess_Cache";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Update_ProductsOfServices Request: " + JsonConvert.SerializeObject(request));
				}
				return Ok(responesData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update_ProductsOfServices} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_TypeProductsOfServices")]
		public async Task<IActionResult> Delete_TypeProductsOfServices(Delete_TypeProductsOfServices request)
		{
			try
			{
				//1. Delete_ProductsOfServices 
				var responesData = await _repository.Delete_TypeProductsOfServices(request);
				if (responesData.ResponseCode == 1)
				{
					var cacheKey = "GetProductOfServicess_Cache";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Delete_ProductsOfServices Request: " + JsonConvert.SerializeObject(request));
				}
				return Ok(responesData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Delete_ProductsOfServices} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpGet("GetList_SreachProductsOfServices")]
		public async Task<IActionResult> GetList_SreachProductsOfServices(GetList_SearchTypeProductsOfServices request)
		{
			try
			{
				var listProOfSer = new List<ProductsOfServicesRespones>();
				//Khóa để lưu trong cache
				var cacheKey = "GetProductOfServicess_Cache";

				//1. Lấy dữ liệu trong caching
				byte[] cachedData = await _cache.GetAsync(cacheKey);

				//1.1 Lưu log request 
				_loggerManager.LogInfo("GetList_SreachProductsOfServices Request:" + JsonConvert.SerializeObject(request));

				//1.2 Nếu Cache có dữ liệu, giải mã trả về Client
				if (cachedData != null) 
				{
					var cachedDataString = Encoding.UTF8.GetString(cachedData);
					listProOfSer = JsonConvert.DeserializeObject<List<ProductsOfServicesRespones>>(cachedDataString);

					//1.3 Lọc dữ liệu khi có request 
					listProOfSer = listProOfSer 
						.Where(ps => 
						(request.ProductsOfServicesID == null || ps.ProductsOfServicesID == request.ProductsOfServicesID) &&
						(string.IsNullOrEmpty(request.ProductsOfServicesName) || ps.ProductsOfServicesName.Contains(request.ProductsOfServicesName
						, StringComparison.OrdinalIgnoreCase)) &&
						(string.IsNullOrEmpty(request.ProductsOfServicesType) || ps.ProductsOfServicesType == request.ProductsOfServicesType)
						).ToList();

					//1.4 Lưu log dữ liệu Supplier trả về trong cache
					_loggerManager.LogInfo("GetList_SreachProductsOfServices cache: " + cachedDataString);

					//1.5 Lưu log kết quả dữ liệu khi có request
					_loggerManager.LogInfo("GetList_SreachProductsOfServices cache Request: " + JsonConvert.SerializeObject(listProOfSer));

					return Ok(listProOfSer);
				}
				else
				{
					//2.Nếu cache không có dữ liệu thì lấy trong db
					var responesData = await _repository.GetList_SearchTypeProductsOfServices(request);

					//2.1 Chuyển đổi dữ liệu lấy từ DB thành danh sách đối tượng ResponseServicess
					listProOfSer = responesData?.Data?.Select(s => new ProductsOfServicesRespones
					{
						ProductsOfServicesID = s.ProductsOfServicesID,
						ProductsOfServicesName = s.ProductsOfServicesName,
						ProductsOfServicesType = s.ProductsOfServicesType,
					}).ToList()?? new List<ProductsOfServicesRespones>();

					//2.2. Chuyển danh sách dịch vụ thành chuỗi JSON để lưu vào cache
					var cachedDataString = JsonConvert.SerializeObject(listProOfSer);
					var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

					//2.3 Cấu hình thời gian hết hạn cho Redis Cache
					DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.4. Lưu log dữ liệu ProOfSer trong db
					_loggerManager.LogInfo("GetList_SreachProductsOfServices db: " + cachedDataString);

					//2.5. Kiểm tra nếu lấy thành công danh sách thì lưu cache
					if (responesData.ResponseCode == 1)
					{
						await _cache.SetAsync(cacheKey, dataToCache, options);
					}
					return Ok(responesData);
				}
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error GetList_SreachProductsOfServices} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
