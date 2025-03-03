using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.DataOpject.RequestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interface
{
	public interface IUserRepository
	{
		//1.Function Tạo Account
		Task<ResponseUser_InsertLoggin> CreateAccount(User_CreateAccount account);

		//2.Function Update User By UserID
		Task<ResponseUser_UpdateLoggin> UpdateUser(User_Update user_Update);

		//3.Function Delete User By UserID
		Task<ResponseUser_DeleteLoggin> DeleteUser(User_Delete user_Delete);

		//4.Function Get list User & Search User by UserName or UserID
		Task<ResponseUserData> GetList_SearchUser(GetList_SearchUser getList_);

		//5.Function get User by ReferralCode
		Task<Users> GetUserIdByReferralCode(string referralCode);

		//6.Update AccumulatedPoints by UserID
		Task UpdateAccumulatedPoints(int userId);

		//7.Function tạo ReferralCode & Ktra trùng
		Task<string> GenerateUniqueReferralCode();

		//8.Get User by UserName
		Task<Users> GetUserByUserName(string UserName);

		//9.Get User by UserID
		Task<Users> GetUserByUserID(int? UserID);
	}
}
