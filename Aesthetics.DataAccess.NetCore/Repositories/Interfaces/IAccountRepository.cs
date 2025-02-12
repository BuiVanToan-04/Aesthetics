using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using XAct.Users;
using BE_102024.DataAces.NetCore.DataOpject.RequestData;
using System.Security.Claims;
using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interface
{
	public interface IAccountRepository
	{
		//1.Hàm đăng nhập
		Task<Users> UserLogin(AccountLoginRequestData requestData);

		//2.Hàm lấy FunctionID qua FunctionCode
		Task<Functions> GetFunctionIDByName(string functionCode);

		//3.Hàm lấy Permisstion qua UserID & FunctionID
		Task<Permission> GetPermisstionUserIDOfFunctionID(int UserID, int functionID);

		//4.Hàm Update RefreshToken
		Task<int> UserUpdate_RefeshToken(int UserID, string RefeshToken, DateTime RefeshTokenExpiryTime);

		//5.Hàm lấy User qua UserName
		Users GetUser_ByUserName(string UserName);

		//6.Hàm lấy User qua UserID 
		Task<Users> User_GetByID(int UserID);

		//7.Hàm lấy tên thiết bị
		Task<string> GetDeviceName();

		//8.Hàm giải mã Token
		Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string? token);
	}
}
