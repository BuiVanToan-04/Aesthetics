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
	}
}
