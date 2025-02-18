using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.RequestData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClinicController : ControllerBase
	{
		private IClinicRepository _clinicRepository;
		public ClinicController(IClinicRepository clinicRepository)
		{
			_clinicRepository = clinicRepository;
		}
		[HttpPost("Insert_Clinic")]
		public async Task<IActionResult> Insert_Clinic(ClinicRequest clinic)
		{
			try
			{
				var responseData = await _clinicRepository.Insert_Clinic(clinic);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex);
			}
		}

		[HttpPost("Update_Clinic")]
		public async Task<IActionResult> Update_Clinic(Update_Clinic update_)
		{
			try
			{
				var responseData = await _clinicRepository.Update_Clinic(update_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex);
			}
		}

		[HttpDelete("Delete_Clinic")]
		public async Task<IActionResult> Delete_Clinic(Delete_Clinic delete_)
		{
			try
			{
				var responseData = await _clinicRepository.Delete_Clinic(delete_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex);
			}
		}

		[HttpGet("GetList_SearchClinic")]
		public async Task<IActionResult> GetList_SearchClinic(GetList_Search getList_)
		{
			try
			{
				var responseData = await _clinicRepository.GetList_SearchClinic(getList_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex);
			}
		}
	}
}
