using Aesthetics.DataAccess.NetCore.CheckConditions;
using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using BE_102024.DataAces.NetCore.DataOpject.RequestData;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAct.Users;
using static System.Collections.Specialized.BitVector32;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class UserRepository : BaseApplicationService, IUserRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		public UserRepository(DB_Context context, IServiceProvider serviceProvider,
			IConfiguration configuration) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<string> GenerateUniqueReferralCode()
		{
			string referralCode;
			bool exists;
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			do
			{
				referralCode = new string(chars.OrderBy(x => random.Next()).Take(5).ToArray());
				exists = await _context.Users.AnyAsync(s => s.ReferralCode == referralCode);
			} while (exists);

			return referralCode;
		}

		public async Task UpdateAccumulatedPoints(int userId)
		{
			try
			{
				var user = await _context.Users.FindAsync(userId);
				if (user != null)
				{
					user.AccumulatedPoints += 10;
					await _context.SaveChangesAsync();
				}
			}
			catch
			{
				throw new Exception($"Không tìm thấy người dùng có mã: {userId}");
			}
		}

		public async Task<Users> GetUserByUserName(string UserName)
		{
			return await _context.Users.Where(s => s.UserName == UserName).FirstOrDefaultAsync();
		}

		public async Task<Users> GetUserIdByReferralCode(string referralCode)
		{
			return await _context.Users.Where(s => s.ReferralCode == referralCode && s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<Users> GetUserByUserID(int? UserID)
		{
			return await _context.Users.Where(s => s.UserID == UserID && s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<ResponseData> CreateAccount(User_CreateAccount account)
		{
			var returnData = new ResponseData();
			try
			{
				if (!Validation.CheckString(account.UserName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Tài khoản không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(account.UserName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Tài khoản chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(account.PassWord))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Mật khẩu không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(account.PassWord))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Mật khẩu chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(account.ReferralCode))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Mã giới thiệu chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (await GetUserByUserName(account.UserName) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "UserName đã tồn tại, Nhập UserName khác!";
					return returnData;
				}

				var passWordHash = Security.EncryptPassWord(account.PassWord);

				//1.Lấy User qua ReferralCode
				var user = await GetUserIdByReferralCode(account.ReferralCode);
				if (user == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Mã giới thiệu người dùng không tồn tại!";
				}

				//2.Tạo mã giới thiệu
				var ReferralCode_User = await GenerateUniqueReferralCode();

				//3.Ngày tạo Account
				var Creation = DateTime.Now;

				//4.Thêm người dùng
				var parameters = new DynamicParameters();
				parameters.Add("@UserName", account.UserName);
				parameters.Add("@PassWord", passWordHash);
				parameters.Add("@Creation", Creation);
				parameters.Add("@ReferralCode", ReferralCode_User);
				await DbConnection.ExecuteAsync("Create_Account", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Tạo Tài Khoản Thành Công!";

				//5.Cập nhật điểm
				if (user != null)
				{
					await UpdateAccumulatedPoints(user.UserID);
				}
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> UpdateUser(User_Update user_Update)
		{
			var returnData = new ResponseData();
			try
			{
				if (user_Update.UserID <= 0 || user_Update.UserID == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"UserID: {user_Update.UserID} không hợp lệ!";
					return returnData;
				}
				if (await GetUserByUserID(user_Update.UserID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"UserID: {user_Update.UserID} không tồn tại!";
					return returnData;
				}

				if (!Validation.CheckXSSInput(user_Update.Email))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Email chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (user_Update.DateBirth > DateTime.Now)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Ngày sinh không hợp lệ!";
					return returnData;
				}

				if (!Validation.CheckXSSInput(user_Update.Sex))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Giới tính chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(user_Update.Addres))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Địa chỉ chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (user_Update.Phone != null)
				{
					if (!Validation.CheckNumber(user_Update.Phone))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Phone không hợp lệ, Phone gồm các số (10-11 số)!";
						return returnData;
					}
				}
				if (!Validation.CheckXSSInput(user_Update.Phone))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Phone chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (user_Update.IDCard != null)
				{
					if (!Validation.CheckNumber(user_Update.IDCard))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "CMND không hợp lệ, CMND gồm các số (10-11 số)!";
						return returnData;
					}
				}
				if (!Validation.CheckXSSInput(user_Update.IDCard))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "CMND chứa kí tự không hợp lệ!";
					return returnData;
				}

				var parameters = new DynamicParameters();
				parameters.Add("@UserID", user_Update.UserID);
				parameters.Add("@Email", user_Update.Email ?? string.Empty);
				parameters.Add("@DateBirth", user_Update.DateBirth);
				parameters.Add("@Sex", user_Update.Sex ?? string.Empty);
				parameters.Add("@Phone", user_Update.Phone ?? string.Empty);
				parameters.Add("@Addres", user_Update.Addres ?? string.Empty);
				parameters.Add("@IDCard", user_Update.IDCard ?? string.Empty);
				var result = await DbConnection.ExecuteAsync("UpdateUser_ByUserID", parameters);
				if (result > 0)
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Cập nhật User có mã: {user_Update.UserID} thành công!";
					return returnData;
				}
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
			}
			return returnData;
		}

		public async Task<ResponseData> DeleteUser(User_Delete user_Delete)
		{
			var returnData = new ResponseData();
			try
			{
				if (user_Delete.UserID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"User: {user_Delete.UserID} không hợp lệ!";
					return returnData;
				}
				if (await GetUserByUserID(user_Delete.UserID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại User: {user_Delete.UserID}!";
					return returnData;
				}

				var user = await _context.Users.FindAsync(user_Delete.UserID);
				if (user != null)
				{
					user.DeleteStatus = 0;
					var result = await _context.SaveChangesAsync();
					if (result > 0)
					{
						returnData.ResponseCode = 1;
						returnData.ResposeMessage = $"Xóa User: {user_Delete.UserID} thành công!";
						return returnData;
					}
				}
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseUserData> GetList_SearchUser(GetList_SearchUser getList_)
		{
			var returnData = new ResponseUserData();
			var listUser = new List<ResponseUser>();
			try
			{
				if (getList_.UserID != null)
				{
					if (getList_.UserID <= 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $":User  {getList_.UserID} không hợp lệ!";
						return returnData;
					}
					if (await GetUserByUserID(getList_.UserID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Danh sách không tồn tại User có mã: {getList_.UserID}!";
						return returnData;
					}
				}

				if (getList_.UserName != null)
				{
					if (!Validation.CheckXSSInput(getList_.UserName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "UserName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}

				var parameters = new DynamicParameters();
				parameters.Add("@UserID", getList_.UserID ?? null);
				parameters.Add("@UserName", getList_.UserName ?? null);
				var result = await DbConnection.QueryAsync<ResponseUser>("GetList_SearchUser ", parameters);

				if (result != null && result.Any())
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Lấy danh sách người dùng thành công!";
					returnData.Data = result.ToList();
					return returnData;
				}
				else
				{
					returnData.ResponseCode = 0;
					returnData.ResposeMessage = "Không tìm thấy người dùng nào.";
					return returnData;
				}
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}
	}
}
