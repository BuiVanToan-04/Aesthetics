using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Impliment
{
	public class UserSessionRepository : BaseApplicationService, IUserSessionRepository
	{
		private DB_Context _context;
		public UserSessionRepository(DB_Context context, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_context = context;
		}

		public async Task<int> DeleleAll_Session(int? UserID)
		{
			try
			{
				var parameters = new DynamicParameters();
				parameters.Add("@UserId", UserID);
				return await DbConnection.ExecuteAsync("UpdateSatusDeleteAll_UserSession", parameters);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error occurred: {ex.Message}");
				return -1;
			}
		}

		public async Task<int> Delele_Session(string? token, int? UserID)
		{
			try
			{
				var parameters = new DynamicParameters();
				parameters.Add("@UserID", UserID);
				parameters.Add("@Token", token);
				return await DbConnection.ExecuteAsync("UpdateSatusDelete_UserSession", parameters);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error occurred: {ex.Message}");
				return -1;
			}
		}

		public async Task<int> Insert_Sesion(DTO.NetCore.DataObject.UserSession session)
		{
			_context.UserSession.Add(session);
			return _context.SaveChanges();
		}
	}
}
