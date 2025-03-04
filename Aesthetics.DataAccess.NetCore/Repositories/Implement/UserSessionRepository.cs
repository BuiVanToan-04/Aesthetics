using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
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
			var userSession_Loggins = new List<UserSession_Loggin>();
			try
			{
				var parameters = new DynamicParameters();
				parameters.Add("@UserId", UserID);
				userSession_Loggins.Add(new UserSession_Loggin
				{
					UserID = UserID,
				});
				return await DbConnection.ExecuteAsync("UpdateSatusDeleteAll_UserSession", parameters);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error DeleleAll_Session Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<int> Delele_Session(string? token, int? UserID)
		{
			var userSession_Loggins = new List<UserSession_Loggin>();
			try
			{
				var parameters = new DynamicParameters();
				parameters.Add("@UserID", UserID);
				parameters.Add("@Token", token);
				userSession_Loggins.Add(new UserSession_Loggin
				{
					UserID = UserID,
					Token = token
				});
				return await DbConnection.ExecuteAsync("UpdateSatusDelete_UserSession", parameters);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Delele_Session Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<int> Insert_Sesion(UserSession session)
		{
			var userSession_Loggins = new List<UserSession_Loggin>();
			try
			{
				_context.UserSession.Add(session);
				userSession_Loggins.Add(new UserSession_Loggin
				{
					UserSessionID = session.UserSessionID,
					UserID = session.UserID,
					Token = session.Token,
					DeviceName = session.DeviceName,
					Ip = session.Ip,
					CreateTime = session.CreateTime,
					DeleteStatus = 1
				});
				return _context.SaveChanges();
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Insert_Sesion Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}
	}
}
