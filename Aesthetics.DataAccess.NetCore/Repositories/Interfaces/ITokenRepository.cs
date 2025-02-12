using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interface
{
	public interface ITokenRepository
	{
		//Tạo Token
		public Task<JwtSecurityToken> CreateToken(List<Claim> authClaims);
		//Tạo chuỗi kí tự  Token
		public Task<string> GenerateRefreshToken();
		//Hàm giải mã Token
		public Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string? token);
	}
}
