using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.RequestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServicessController : ControllerBase
	{
		private IServicessRepository _servicess;
		public ServicessController(IServicessRepository servicess)
		{
			_servicess = servicess;
		}

		[HttpPost("Insert_Servicess")]
		public async Task<IActionResult> Insert_Servicess(ServicessRequest servicess)
		{
			try
			{
				var responseData = await _servicess.Insert_Servicess(servicess);
				return Ok(responseData);
			}
			catch (Exception ex) 
			{
				return Ok(ex.StackTrace);
			}
		}
		[HttpPost("Update_Servicess")]
		public async Task<IActionResult> Update_Servicess(Update_Servicess servicess)
		{
			try
			{
				var responseData = await _servicess.Update_Servicess(servicess);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpPost("ExportServicessToExcel")]
		public async Task<IActionResult> ExportServicessToExcel(ExportExcel filePath)
		{
			try
			{
				var responseData = await _servicess.ExportServicessToExcel(filePath);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_Servicess")]
		public async Task<IActionResult> Delete_Servicess(Delete_Servicess servicess)
		{
			try
			{
				var responseData = await _servicess.Delete_Servicess(servicess);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}
		[HttpGet("GetList_SearchServicess")]
		public async Task<IActionResult> GetList_SearchServicess(GetList_SearchServicess servicess)
		{
			try
			{
				var responseData = await _servicess.GetList_SearchServicess(servicess);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}
	}
}
