using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using BE_102024.DataAces.NetCore.DataOpject.RequestData;
using BE_102024.DataAces.NetCore.DataOpject.TokenModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASP_NetCore_Aesthetics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private IAccountRepository _accountRepository;
		private IUserSessionRepository _userSession;
		private IConfiguration _configuration;
		private DB_Context _context;
		private readonly IDistributedCache _cache;
		public AuthenticationController(IAccountRepository accountRepository,
			IConfiguration configuration, DB_Context context, IDistributedCache cache, IUserSessionRepository userSession)
		{
			_accountRepository = accountRepository;
			_configuration = configuration;
			_context = context;
			_cache = cache;
			_userSession = userSession;
		}

		[HttpPost("Login_Account")]
		public async Task<IActionResult> Login_Account(AccountLoginRequestData loginRequestData)
		{
			var responseData = new UserLoginResponseData();
			try
			{
				var user = await _accountRepository.UserLogin(loginRequestData);
				if (user == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Đăng Nhập Thất Bại, Vui Lòng Kiểm Tra Lại UserName || PassWord!";
					return Ok(responseData);
				}
				//Tạo Token
				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.PrimarySid, user.UserID.ToString()),
					new Claim(ClaimTypes.Role, user.TypePerson ?? string.Empty),
					new Claim(ClaimTypes.Authentication, user.RefeshToken ?? string.Empty)
				};

				//Lưu RefreshToken
				var newToken = await _accountRepository.CreateToken(authClaims);
				_ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
				var refeshToken = await _accountRepository.GenerateRefreshToken();
				await _accountRepository.UserUpdate_RefeshToken(user.UserID, refeshToken, DateTime.Now.AddDays(refreshTokenValidityInDays));

				//Lấy tên thiết bị
				var DeviceName = await _accountRepository.GetDeviceName();

				//Lấy địa chỉ IP
				var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

				//Lưu vào RedisCaching
				var cachKey = "User_" + user.UserID + "-" + DeviceName;

				//Lưu vào db
				var user_Session = new Aesthetics.DTO.NetCore.DataObject.Model.UserSession
				{
					UserID = user.UserID,
					Token = new JwtSecurityTokenHandler().WriteToken(newToken),
					DeviceName = DeviceName,
					Ip = remoteIpAddress.ToString(),
					CreateTime = DateTime.Now,
					DeleteStatus = 1
				};
				await _userSession.Insert_Sesion(user_Session);

				//Xét vào caching
				var user_SessionCach = new Aesthetics.DTO.NetCore.DataObject.Model.UserSession
				{
					UserID = user.UserID,
					Token = new JwtSecurityTokenHandler().WriteToken(newToken),
					DeviceName = DeviceName,
					Ip = remoteIpAddress.ToString(),
					CreateTime = DateTime.Now
				};
				//1.Chuyển thành dạng Json
				var dataCachingJson = JsonConvert.SerializeObject(user_SessionCach);

				//2.Chuyển dataCachingJson thành byte 
				var dataToCache = Encoding.UTF8.GetBytes(dataCachingJson);

				//3.Xét thời gian sống của Token trong Caching 
				DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
			   .SetAbsoluteExpiration(DateTime.Now.AddMinutes(3));

				_cache.Set(cachKey, dataToCache, options);

				//Trả về token & thông tin
				responseData.ResponseCode = 1;
				responseData.ResposeMessage = "Đăng nhập thành công!";
				responseData.DeviceName = DeviceName;
				responseData.UserID = user.UserID;
				responseData.Token = new JwtSecurityTokenHandler().WriteToken(newToken);
				responseData.RefreshToken = refeshToken;
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				responseData.ResponseCode = -99;
				responseData.ResposeMessage = ex.Message;
				return Ok(responseData);
			}
		}

		[HttpPost("Check_Token")]
		public async Task<IActionResult> Check_Token(CheckTokenRequestData requestData)
		{
			var responeData = new CheckTokenReponsData();
			try
			{
				//1.Kiểm tra thời hạn của token 
				var cachKey = "User_" + requestData.UserID + "-" + requestData.DeviceName;
				byte[] cacheData = await _cache.GetAsync(cachKey);
				if (cacheData == null)
				{
					//2. Kiểm tra thời hạn RefreshToken Expried Time:
					var userDetail = await _accountRepository.User_GetByID(requestData.UserID);
					if (userDetail == null)
					{
						responeData.ResponseCode = -1;
						responeData.ResposeMessage = $"Token của User: {requestData.UserID} không tồn tại!";
						return Ok(responeData);
					}
					//2.1 Hết hạn => đăng nhập lại
					if (userDetail.TokenExprired < DateTime.Now)
					{
						responeData.ResponseCode = -1;
						responeData.ResposeMessage = "Token hết hạn, Vui lòng đăng nhập lại!";
						return Ok(responeData);
					}

					//2.2 Còn hạn => Tạo Token mới:
					//2.2.1 Giải mã Token truyền lên để lấy claims
					var principal = await _accountRepository.GetPrincipalFromExpiredToken(requestData.AccessToken);
					if (principal == null)
					{
						responeData.ResponseCode = -1;
						responeData.ResposeMessage = "Token không hợp lệ!";
						return Ok(responeData);
					}
					//2.2.1 Check RefreshToken và ngày hết hạn
					var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(principal.FindFirst("exp").Value));
					DateTime result = exp.UtcDateTime.AddHours(7);
					string userName = principal.Identity.Name;

					//Gọi db lấy theo UserName
					var user = _accountRepository.GetUser_ByUserName(userName);

					//Nếu ngày hết hạn < thời gian hiện tại || RefreshToken truyền lên khác RefreshToken trong db
					if (user == null || user.RefeshToken != requestData.RefreshToken || user.TokenExprired <= DateTime.Now)
					{
						responeData.ResponseCode = -1;
						responeData.ResposeMessage = "Token không hợp lệ!";
						return Ok(responeData);
					}
					var newToken = await _accountRepository.CreateToken(principal.Claims.ToList());
					var newRefreshToken = await _accountRepository.GenerateRefreshToken();

					//3. Tạo Token và RefreshToken mới
					_ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
					await _accountRepository.UserUpdate_RefeshToken(user.UserID, newRefreshToken, DateTime.Now.AddDays(refreshTokenValidityInDays));
					responeData.ResponseCode = 1;
					responeData.ResposeMessage = "Tạo mới Token thành công";
					responeData.Token = new JwtSecurityTokenHandler().WriteToken(newToken);
					responeData.RefreshToken = newRefreshToken;
					return Ok(responeData);
				}
				responeData.ResponseCode = 1;
				responeData.ResposeMessage = "Token vẫn còn thời hạn hoạt động!";
				return Ok(responeData);
			}
			catch (Exception ex)
			{
				responeData.ResponseCode = -99;
				responeData.ResposeMessage = ex.Message;
				return Ok(responeData);
			}
		}

		[HttpPost("Refresh_Token")]
		public async Task<IActionResult> Refresh_Token(TokenModel tokenModel)
		{
			var responseData = new UserLoginResponseData();
			try
			{
				if (tokenModel == null || string.IsNullOrEmpty(tokenModel.AccessToken)
					|| string.IsNullOrEmpty(tokenModel.RefreshToken))
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Dữ liệu đầu vào không hợp lệ";
					return Ok(responseData);
				}

				if (tokenModel == null)
				{
					return BadRequest("Yêu cầu Token không hợp lệ");
				}
				string? accessToken = tokenModel.AccessToken;
				string? refreshToken = tokenModel.RefreshToken;

				//Bước 1: Giai mã Token truyền lên để lấy claims
				var principal = await _accountRepository.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
				if (principal == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Token không hợp lệ";
					return Ok(responseData);
				}

				//Bước 2: Check RefresToken và ngày hết hạn
				var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(principal.FindFirst("exp").Value));
				DateTime result = exp.UtcDateTime.AddHours(7);
				string userName = principal.Identity.Name;

				//Gọi dataBase để lấy theo userName
				var user = _accountRepository.GetUser_ByUserName(userName);

				//Nếu ngày hết hạn < thời gian hiện tại || RefreshToken truyền lên khác RefreshToken
				if (user == null || user.RefeshToken != refreshToken || user.TokenExprired <= DateTime.Now)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Token không hợp lệ";
					return Ok(responseData);
				}

				var newToken = await _accountRepository.CreateToken(principal.Claims.ToList());
				var newRefeshToken = await _accountRepository.GenerateRefreshToken();

				//Bước 3: Tạo Token mới và RefresToken mới
				_ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
				await _accountRepository.UserUpdate_RefeshToken(user.UserID, newRefeshToken, DateTime.Now.AddDays(refreshTokenValidityInDays));

				responseData.ResponseCode = 1;
				responseData.ResposeMessage = "Tạo mới RefreshToken thành công";
				responseData.Token = new JwtSecurityTokenHandler().WriteToken(newToken);
				responseData.RefreshToken = newRefeshToken;
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				return Ok(ex);
			}
		}

		[HttpPost("LogOut_Account")]
		public async Task<IActionResult> LogOut_Account(TokenLogOutModel token)
		{
			var responseData = new UserLogOutResponseData();
			try
			{
				if (token == null || string.IsNullOrEmpty(token.AccessToken))
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Token không hợp lệ";
					return Ok(responseData);
				}
				//1.Xóa Token trong Caching:
				//1.1 Giải mã Token truyền lên
				var principal = await _accountRepository.GetPrincipalFromExpiredToken(token.AccessToken);
				if (principal == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Token không hợp lệ";
					return Ok(responseData);
				}

				string userName = principal.Identity.Name;
				var user = _accountRepository.GetUser_ByUserName(userName);
				if (user == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Token truyền lên không hợp lệ";
					return Ok(responseData);
				}
				//Lấy dữ liệu từ Redis => LogOut trên 1 thiết bị
				var DeviceName = await _accountRepository.GetDeviceName();
				var cachKey = "User_" + user.UserID + "-" + DeviceName;
				_cache.Remove(cachKey);
				await _userSession.Delele_Session(token.AccessToken, user.UserID);
				responseData.ResponseCode = 1;
				responseData.ResposeMessage = "Đăng xuất thành công";
				return Ok(responseData);

			}
			catch (Exception ex)
			{
				responseData.ResponseCode = -99;
				responseData.ResposeMessage = ex.Message;
				return Ok(responseData);
			}
		}

		[HttpPost("LogOutAll_Account")]
		public async Task<IActionResult> LogOutAll_Account(TokenLogOutModel tokenLogOut)
		{
			var responseData = new UserLogOutResponseData();
			try
			{
				if (tokenLogOut == null || string.IsNullOrEmpty(tokenLogOut.AccessToken))
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Đăng nhập thất bại, Kiểm tra lại UserName, Password";
					return Ok(responseData);
				}
				//Thực hiện xóa Token trong Caching:

				//Bước 1: Giải mã token truyền lên
				var principal = await _accountRepository.GetPrincipalFromExpiredToken(tokenLogOut.AccessToken);
				if (principal == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Token không hợp lệ";
					return Ok(responseData);
				}

				//Bước 2: Check RefreshToken và ngày hết hạn 
				string userName = principal.Identity.Name;
				var user = _accountRepository.GetUser_ByUserName(userName);
				if (user == null)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Token truyền lên không hợp lệ";
					return Ok(responseData);
				}

				//Bước 3: Lấy dữ liệu từ RedisCaching và LogOut trên mọi thiết bị 
				var DeviceName = await _accountRepository.GetDeviceName();
				var cachKey = "User_" + user.UserID + "-" + DeviceName;
				using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true"))
				{
					IDatabase db = redis.GetDatabase();
					var server = redis.GetServer("localhost", 6379);

					// Lấy tất cả các keys từ Redis theo pattern User_*
					var keys = server.Keys(pattern: "User_*");

					foreach (var key in keys)
					{
						// Kiểm tra nếu key khớp với user.UserID
						if (key.ToString().Contains(user.UserID.ToString()))
						{
							Console.WriteLine($"Deleting key: {key}");

							// Xóa key khỏi Redis
							db.KeyDelete(key);
						}
					}
					//Xóa trong db
					await _userSession.DeleleAll_Session(user.UserID);
					Console.WriteLine($"Đã đang xuất tất cả các thiết bị của {user.UserID}");
				}
				responseData.ResponseCode = 1;
				responseData.ResposeMessage = "Đăng xuất thành công";
				return Ok(responseData);
			}
			catch (Exception ex)
			{
				responseData.ResponseCode = -99;
				responseData.ResposeMessage = ex.Message;
				return Ok(responseData);
			}
		}
	}
}
