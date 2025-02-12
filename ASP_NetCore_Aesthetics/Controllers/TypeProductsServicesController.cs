using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.RequestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TypeProductsServicesController : ControllerBase
	{
		private ITypeProductsOfServicesRepository _repository;
		public TypeProductsServicesController(ITypeProductsOfServicesRepository repository)
		{
			_repository = repository;
		}

		[HttpPost("Insert_TypeProductsOfServices")]
		public async Task<IActionResult> Insert_TypeProductsOfServices(TypeProductsOfServicesRequest request)
		{
			try
			{
				var responesData = await _repository.Insert_TypeProductsOfServices(request);
				return Ok(responesData);
			}
			catch (Exception ex) 
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpPost("Update_TypeProductsOfServices")]
		public async Task<IActionResult> Update_TypeProductsOfServices(Update_TypeProductsOfServices request)
		{
			try
			{
				var responesData = await _repository.Update_TypeProductsOfServices(request);
				return Ok(responesData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpDelete("Delete_TypeProductsOfServices")]
		public async Task<IActionResult> Delete_TypeProductsOfServices(Delete_TypeProductsOfServices request)
		{
			try
			{
				var responesData = await _repository.Delete_TypeProductsOfServices(request);
				return Ok(responesData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpGet("GetList_SreachProductsOfServices")]
		public async Task<IActionResult> GetList_SreachProductsOfServices(GetList_SearchTypeProductsOfServices request)
		{
			try
			{
				var responesData = await _repository.GetList_SearchTypeProductsOfServices(request);
				return Ok(responesData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}
	}
}
