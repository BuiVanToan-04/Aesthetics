using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using ASP_NetCore_Aesthetics.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private IUserRepository _user;
		public UsersController(IUserRepository user)
		{
			_user = user;
		}
		[HttpPost("Create_Account")]
		public async Task<IActionResult> Create_Account(User_CreateAccount account)
		{
			try
			{
				var responseData = await _user.CreateAccount(account);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[Filter_Authorization("Update_User", "UPDATE")]
		[HttpPost("Update_User")]
		public async Task<IActionResult> Update_User(User_Update user_)
		{
			try
			{
				var responseData = await _user.UpdateUser(user_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpDelete("Delete_User")]
		public async Task<IActionResult> Delete_User(User_Delete UserID)
		{
			try
			{
				var responseData = await _user.DeleteUser(UserID);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.StackTrace);
			}
		}

		[HttpGet("GetList_SearchUser")]
		public async Task<IActionResult> GetList_SearchUser(GetList_SearchUser getList_)
		{
			try
			{
				var responseData = await _user.GetList_SearchUser(getList_);
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex.Message);
			}
		}
	}
}
