using Aesthetics.DataAccess.NetCore.CheckConditions;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.DataObject;
using BE_102024.DataAces.NetCore.DataOpject.RequestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XAct.Users;

namespace Aesthetics.DataAccess.NetCore.Repositories.Impliment
{
	public class AccountRepository : IAccountRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		public AccountRepository(DB_Context context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<string> GetDeviceName()
		{
			return Environment.MachineName;
		}

		public async Task<Functions> GetFunctionIDByName(string functionCode)
		{
			return await _context.Functions.Where(s => s.FunctionCode == functionCode).FirstOrDefaultAsync();
		}

		public async Task<Permission> GetPermisstionUserIDOfFunctionID(int UserID, int functionID)
		{
			return _context.Permission.ToList().Where(s => s.UserID == UserID && s.FunctionID == functionID).FirstOrDefault();
		}

		public async Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string? token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
				ValidateLifetime = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
			if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("Invalid token");

			return principal;
		}

		public Users GetUser_ByUserName(string UserName)
		{
			return _context.Users.ToList().Where(s => s.UserName == UserName).FirstOrDefault();
		}

		public async Task<Users> UserLogin(AccountLoginRequestData requestData)
		{
			try
			{
				var passwordHash = Security.EncryptPassWord(requestData.Password);
				var user = _context.Users.ToList().Where(s => s.UserName == requestData.UserName
				&& s.PassWord == passwordHash && s.DeleteStatus == 1).FirstOrDefault();
				return user;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<int> UserUpdate_RefeshToken(int UserID, string RefeshToken, DateTime RefeshTokenExpiryTime)
		{
			var user = _context.Users.Where(s => s.UserID == UserID).FirstOrDefault();
			if (user == null)
			{
				return -1;
			}
			user.RefeshToken = RefeshToken;
			user.TokenExprired = RefeshTokenExpiryTime;
			_context.Users.Update(user);
			_context.SaveChanges();
			return -1;
		}

		public async Task<Users> User_GetByID(int UserID)
		{
			return _context.Users.ToList().Where(s => s.UserID == UserID).FirstOrDefault();
		}
	}
}
