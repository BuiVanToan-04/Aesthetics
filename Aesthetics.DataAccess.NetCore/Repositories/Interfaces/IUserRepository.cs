using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject;
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
		Task<ResponseData> CreateAccount(User_CreateAccount account);

		//2.Function get User by ReferralCode
		Task<Users> GetUserIdByReferralCode(string referralCode);

		//3.Update AccumulatedPoints by UserID
		Task UpdateAccumulatedPoints(int userId);

		//4.Function tạo ReferralCode & Ktra trùng
		Task<string> GenerateUniqueReferralCode();

		//5.Get User by UserName
		Task<Users> GetUserByUserName(string UserName);

		//6.Get User by UserID
		Task<Users> GetUserByUserID(int? UserID);

		//7.Function Update User By UserID
		Task<ResponseData> UpdateUser(User_Update user_Update);

		//8.Function Delete User By UserID
		Task<ResponseData> DeleteUser(User_Delete user_Delete);

		//9.Function Get list User & Search User by UserName or UserID
		Task<ResponseUserData> GetList_SearchUser(GetList_SearchUser getList_);
	}
}
