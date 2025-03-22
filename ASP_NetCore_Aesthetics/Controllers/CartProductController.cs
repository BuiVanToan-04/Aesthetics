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
    public class CartProductController : ControllerBase
    {
        private ICartProductRepository _cartProductRepository;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public CartProductController(ICartProductRepository cartProductRepository, IDistributedCache cache,
			ILoggerManager loggerManager)
		{
			_cartProductRepository = cartProductRepository;
			_cache = cache;
			_loggerManager = loggerManager;
		}

		[HttpPost("Insert_CartProduct")]
		public async Task<IActionResult> Insert_CartProduct(Cart_ProductRequest insert_)
		{
			try
			{
				//1.Insert_CartProduct 
				var responseData = await _cartProductRepository.Insert_CartProduct(insert_);
				//2. Lưu log request
				_loggerManager.LogInfo("Insert_CartProduct Request: " + JsonConvert.SerializeObject(insert_));
				//3. Lưu log data response
				_loggerManager.LogInfo("Insert_CartProduct Response data: " + JsonConvert.SerializeObject(responseData.cartProduct_Loggins));
				//if (responseData.ResponseCode == 1)
				//{
				//	var cacheKey = "GetSupplier_Cache";
				//	await _cache.RemoveAsync(cacheKey);
				//}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Insert_CartProduct} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpPost("Update_CartProduct")]
		public async Task<IActionResult> Update_CartProduct(Update_Cart_ProductRequest update_)
		{
			try
			{
				//1.Update_CartProduct 
				var responseData = await _cartProductRepository.Update_CartProduct(update_);
				//2. Lưu log request
				_loggerManager.LogInfo("Update_CartProduct Request: " + JsonConvert.SerializeObject(update_));
				//3. Lưu log data response
				_loggerManager.LogInfo("Update_CartProduct Response data: " + JsonConvert.SerializeObject(responseData.cartProduct_Loggins));
				//if (responseData.ResponseCode == 1)
				//{
				//	var cacheKey = "GetSupplier_Cache";
				//	await _cache.RemoveAsync(cacheKey);
				//}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update_CartProduct} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_CartProduct")]
		public async Task<IActionResult> Delete_CartProduct(Delete_Cart_ProductRequest delete_)
		{
			try
			{
				//1.Delete_CartProduct 
				var responseData = await _cartProductRepository.Delete_CartProduct(delete_);
				//2. Lưu log request
				_loggerManager.LogInfo("Delete_CartProduct Request: " + JsonConvert.SerializeObject(delete_));
				//3. Lưu log data response
				_loggerManager.LogInfo("Delete_CartProduct Response data: " + JsonConvert.SerializeObject(responseData.cartProduct_Loggins));
				//if (responseData.ResponseCode == 1)
				//{
				//	var cacheKey = "GetSupplier_Cache";
				//	await _cache.RemoveAsync(cacheKey);
				//}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Delete_CartProduct} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpGet("GetList_SearchCartProduct")]
		public async Task<IActionResult> GetList_SearchCartProduct(GetList_SearchCart_ProductRequest getList_SearchCart_)
		{
			try
			{
				//1.Delete_CartProduct 
				var responseData = await _cartProductRepository.GetList_SearchCartProduct(getList_SearchCart_);
				//2. Lưu log request
				_loggerManager.LogInfo("GetList_SearchCartProduct Request: " + JsonConvert.SerializeObject(getList_SearchCart_));
				//3. Lưu log data response
				_loggerManager.LogInfo("GetList_SearchCartProduct Response data: " + JsonConvert.SerializeObject(responseData.cartProduct_Loggins));
				//if (responseData.ResponseCode == 1)
				//{
				//	var cacheKey = "GetSupplier_Cache";
				//	await _cache.RemoveAsync(cacheKey);
				//}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error GetList_SearchCartProduct} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
