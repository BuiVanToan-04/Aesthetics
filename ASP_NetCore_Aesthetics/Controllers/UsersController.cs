using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore;
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
	public class UsersController : ControllerBase
	{
		private IUserRepository _user;
		private readonly IDistributedCache _cache;
		private readonly ILoggerManager _loggerManager;
		public UsersController(IUserRepository user, IDistributedCache cache, ILoggerManager loggerManager)
		{
			_user = user;
			_cache = cache;
			_loggerManager = loggerManager;
		}
		[HttpPost("Create_Account")]
		public async Task<IActionResult> Create_Account(User_CreateAccount account)
		{
			try
			{
				//1. Create_Account
				var responseData = await _user.CreateAccount(account);
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetUser_Cache";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Create_Account Request: " + JsonConvert.SerializeObject(account));
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Create_Account} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[Filter_Authorization("Update_User", "UPDATE")]
		[HttpPost("Update_User")]
		public async Task<IActionResult> Update_User(User_Update user_)
		{
			try
			{
				//1. Update_User
				var responseData = await _user.UpdateUser(user_);
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetUser_Cache";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Update_User Request: " + JsonConvert.SerializeObject(user_));
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Update_User} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpDelete("Delete_User")]
		public async Task<IActionResult> Delete_User(User_Delete delete_)
		{
			try
			{
				//1. Delete_User
				var responseData = await _user.DeleteUser(delete_);
				if (responseData.ResponseCode == 1)
				{
					var cacheKey = "GetUser_Cache";
					await _cache.RemoveAsync(cacheKey);
					//2. Lưu log
					_loggerManager.LogInfo("Delete_User Request: " + JsonConvert.SerializeObject(delete_));
				}
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				_loggerManager.LogError("{Error Delete_User} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}

		[HttpGet("GetList_SearchUser")]
		public async Task<IActionResult> GetList_SearchUser(GetList_SearchUser getList_)
		{
			try
			{
				var listUser = new List<ResponseUser>();
				//Khóa để lưu dữ liệu trong Redis caching
				var cacheKey = "GetUser_Cache";

				//1. Lấy dữ liệu trong Redis caching
				byte[] cachedData = await _cache.GetAsync(cacheKey);

				//1.1 Lưu log request
				_loggerManager.LogInfo("GetList_SearchUser Request: " + JsonConvert.SerializeObject(getList_));

				//1.2 Nếu Cache có dữ liệu, giải mã trả về Client
				if (cachedData != null)
				{
					var cachedDataString = Encoding.UTF8.GetString(cachedData);
					listUser = JsonConvert.DeserializeObject<List<ResponseUser>>(cachedDataString);

					//1.3 Lọc dữ liệu khi có request 
					listUser = listUser
						.Where(s =>
						(getList_.UserID == null || s.UserID == getList_.UserID) &&
						(string.IsNullOrEmpty(getList_.UserName) || s.UserName.Contains(getList_.UserName, StringComparison.OrdinalIgnoreCase))
						).ToList();

					//1.4 Lưu log dữ liệu Supplier trả về trong cache
					_loggerManager.LogInfo("GetList_SearchUser cache: " + cachedDataString);

					//1.5 Lưu log kết quả dữ liệu khi có request
					_loggerManager.LogInfo("GetList_SearchUser cache Request: " + JsonConvert.SerializeObject(listUser));

					return Ok(listUser);
				}
				else
				{
					//2. Nếu cache không có dữ liệu gọi vào db
					var responseData = await _user.GetList_SearchUser(getList_);

					//2.1 Chuyển đổi dữ liệu lấy từ DB thành danh sách đối tượng ResponseServicess
					listUser = responseData?.Data?.Select(s => new ResponseUser
					{
						UserID = s.UserID,
						UserName = s.UserName,
						Email = s.Email,
						DateBirth = s.DateBirth,
						Sex = s.Sex,
						Phone = s.Phone,
						Addres = s.Addres,
						IDCard = s.IDCard,
						TypePerson = s.TypePerson,
					}).ToList() ?? new List<ResponseUser>();

					//2.2. Chuyển danh sách dịch vụ thành chuỗi JSON để lưu vào cache
					var cachedDataString = JsonConvert.SerializeObject(listUser);
					var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

					//2.3 Cấu hình thời gian hết hạn cho Redis Cache
					DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
						.SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
						.SetSlidingExpiration(TimeSpan.FromMinutes(3));

					//2.4. Lưu log dữ liệu User trong db
					_loggerManager.LogInfo("GetList_SearchUser db: " + cachedDataString);

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
				_loggerManager.LogError("{Error GetList_SearchUser} Message: " + ex.Message +
					"|" + "Stack Trace: " + ex.StackTrace);
				return Ok(ex.Message);
			}
		}
	}
}
