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
    public class ProductsController : ControllerBase
    {
        private IProductsRepository _productsRepository;
        private readonly ILoggerManager _logger;
		private readonly IDistributedCache _cache;
		public ProductsController(IProductsRepository productsRepository, 
			ILoggerManager logger, IDistributedCache cache)
		{
			_productsRepository = productsRepository;
			_logger = logger;
			_cache = cache;
		}
		[HttpPost("Insert_Products")]
		public async Task<IActionResult> Insert_Products(ProductRequest product_)
		{
			try
			{
				//1. Insert Products
				var responseData = await _productsRepository.Insert_Products(product_);
				//2. Lưu log request
				_logger.LogInfo("Insert Products Request: " + JsonConvert.SerializeObject(product_));
				//3. Lưu log data products
				_logger.LogInfo("Insert Servicess Response data : " + JsonConvert.SerializeObject(responseData.products_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetList_SearchProducts_Caching";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_logger.LogError("{Error Insert Servicess} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("Update_Products")]
		public async Task<IActionResult> Update_Products(Update_Products product_)
		{
			try
			{
				//1. Update_Products
				var responseData = await _productsRepository.Update_Products(product_);
				//2. Lưu log request
				_logger.LogInfo("Update_Products Request: " + JsonConvert.SerializeObject(product_));
				//3. Lưu log data products
				_logger.LogInfo("Update_Products Response data : " + JsonConvert.SerializeObject(responseData.products_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetList_SearchProducts_Caching";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_logger.LogError("{Error Update_Products} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_Products")]
		public async Task<IActionResult> Delete_Products(Delete_Products product_)
		{
			try
			{
				//1. Update_Products
				var responseData = await _productsRepository.Delete_Products(product_);
				//2. Lưu log request
				_logger.LogInfo("Delete_Products Request: " + JsonConvert.SerializeObject(product_));
				//3. Lưu log data products
				_logger.LogInfo("Delete_Products Response data : " + JsonConvert.SerializeObject(responseData.products_Loggins));
				//4. Lưu log data comment
				_logger.LogInfo("Delete_Comment Response data : " + JsonConvert.SerializeObject(responseData.comment_Loggins));
				//5. Lưu log data cartProducts
				_logger.LogInfo("Delete_CartProducts Response data : " + JsonConvert.SerializeObject(responseData.cartProducts_Loggins));
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetList_SearchProducts_Caching";
					await _cache.RemoveAsync(cacheKey);
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_logger.LogError("{Error Delete_Products} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("ExportProductsToExcel")]
		public async Task<IActionResult> ExportProductsToExcel(ExportExcel filePath)
		{
			try
			{
				//1.Export excel
				var responseData = await _productsRepository.ExportProductsToExcel(filePath);
				//2. Lưu log
				_logger.LogInfo("ExportProductsToExcel Servicess Request: " + JsonConvert.SerializeObject(filePath));
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_logger.LogError("{Error ExportProductsToExcel} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}


		[HttpGet("GetList_SearchProducts")]
		public async Task<IActionResult> GetList_SearchProducts(GetList_SearchProducts getList_)
		{
			try
			{
				var listProducts = new List<ResponseGetListProducts>();

				//Khóa lưu dữ liệu trong cache
				var cacheKey = "GetList_SearchProducts_Caching";

				//1. Lấy dữ liệu trong Redis caching
				byte[] cacheData = await _cache.GetAsync(cacheKey);

				//1.1 Lưu log request get list products
				_logger.LogInfo("GetList_SearchProducts Request: " + JsonConvert.SerializeObject(getList_));

				//1.2 Nếu Cache có dữ liệu, giải mã trả về Client
				if (cacheData != null)
				{
					var cachedDataString = Encoding.UTF8.GetString(cacheData);
					listProducts = JsonConvert.DeserializeObject<List<ResponseGetListProducts>>(cachedDataString);

					//1.3. Lọc dữ liệu khi có request 
					listProducts = listProducts.Where(s =>
						(getList_.ProductID == null || s.ProductID == getList_.ProductID) &&
						(getList_.ProductsOfServicesName == null || s.ProductsOfServicesName == getList_.ProductsOfServicesName) &&
						(getList_.SupplierName == null || s.SupplierName == getList_.SupplierName) &&
						(getList_.ProductName == null || s.ProductName.Contains(getList_.ProductName, StringComparison.OrdinalIgnoreCase))
					).ToList();

					//1.4. Lưu log dữ liệu products trả về trong cache
					_logger.LogInfo("GetList_SearchProducts cache: " + cachedDataString);

					//1.5 Lưu log kết quả dữ liệu khi có request
					_logger.LogInfo("GetList_SearchProducts cache Request: " + JsonConvert.SerializeObject(listProducts));
					return Ok(listProducts);
				}
				else
				{
					//2. Nếu Redis Cache không có dữ liệu, gọi vào database để lấy dữ liệu mới
					var responseData = await _productsRepository.GetList_SearchProducts(getList_);

					//2.1. Chuyển đổi dữ liệu lấy từ DB thành danh sách đối tượng ResponseProducts
					listProducts = responseData?.Data?.ToList();

					//2.2. Chuyển danh sách dịch vụ thành chuỗi JSON để lưu vào cache
					var cachedDataString = JsonConvert.SerializeObject(listProducts);
					var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

					//2.3 Cấu hình thời gian hết hạn cho Redis Cache
					DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.4. Lưu log dữ liệu products trong db
					_logger.LogInfo("GetList_SearchProducts db: " + cachedDataString);

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
				_logger.LogError("{Error GetList_SearchProducts} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
