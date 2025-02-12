using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using ASP_NetCore_Aesthetics.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private ISupplierRepository _supplierRepository;
        public SupplierController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
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
