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
                var responseData = await _supplierRepository.Insert_Supplier(supplier);
                return Ok(responseData);
            }
            catch (Exception ex) 
            {
                return Ok(ex.StackTrace);
            }
        }

		[HttpPost("Update_Supplier")]
		public async Task<IActionResult> Update_Supplier(Update_Supplier supplier)
		{
			try
			{
				var responseData = await _supplierRepository.Update_Supplier(supplier);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpDelete("Delete_Supplier")]
		public async Task<IActionResult> Delete_Supplier(Delete_Supplier supplier)
		{
			try
			{
				var responseData = await _supplierRepository.Delete_Supplier(supplier);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[Filter_Authorization("Update_User", "UPDATE")]
		[HttpGet("GetList_SearchSupplier")]
		public async Task<IActionResult> GetList_SearchSupplier(GetList_SearchSupplier supplier)
		{
			var listSupplier = new List<Supplier>();
			//Khóa để lưu dữ liệu trong Redis caching
			var cacheKey = "GetSupplier_Cache";

			//1. Lấy dữ liệu trong Redis caching
			byte[] cachedData = await _cache.GetAsync(cacheKey);

			//1.1 Lưu log request
			_loggerManager.LogInfo("GetList_SearchSupplier: " + JsonConvert.SerializeObject(supplier));

			//1.2 Nếu Cache có dữ liệu, giải mã trả về Client
			if (cachedData != null)
			{
				var cachedDataString = Encoding.UTF8.GetString(cachedData);
				listSupplier = JsonConvert.DeserializeObject<List<Supplier>>(cachedDataString);

				//1.3 Lọc dữ liệu khi có request 
				listSupplier = listSupplier
					.Where(s => 
					(s.SupplierID != null || s.SupplierID == supplier.SupplierID) &&
					(string.IsNullOrEmpty(supplier.SupplierName) || s.SupplierName.Contains(supplier.SupplierName, StringComparison.OrdinalIgnoreCase))
					).ToList();

				//1.4 Lưu log dữ liệu Supplier trả về trong cache
				_loggerManager.LogInfo("GetList_SearchSupplier cache: " + cachedDataString);

				return Ok(listSupplier);
			}
			try
			{
				var responseData = await _supplierRepository.GetList_SearchSupplier(supplier);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}
	}
}
