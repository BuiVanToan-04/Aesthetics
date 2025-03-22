using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class Clinic_StaffRepository : BaseApplicationService,IClinic_StaffRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		private IClinicRepository _clinicRepository;
		private IUserRepository _userRepository;
		public Clinic_StaffRepository(DB_Context context, IServiceProvider serviceProvider
			, IConfiguration configuration, IClinicRepository clinicRepository,
			IUserRepository userRepository) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_clinicRepository = clinicRepository;
			_userRepository = userRepository;
		}
		public async Task<Response_ClinicStaff_Loggin> Insert_Clinic_Staff(Clinic_StaffRequest insert_)
		{
			var returnData = new Response_ClinicStaff_Loggin();
			var clinic_Staff_Loggins = new List<Clinic_Staff_Loggin>();
			try
			{
				if (await _clinicRepository.GetClinicByID(insert_.ClinicID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ClinicID không tồn tại. Vui lòng nhập ClinicID khác!";
					return returnData;
				}
				if (await _userRepository.GetUserByUserID(insert_.UserID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "UserID không tồn tại. Vui lòng nhập UserID khác!";
					return returnData;
				}

				var clinic_staff = new Clinic_Staff()
				{
					ClinicID = insert_.ClinicID,
					UserID = insert_.UserID,
				};
				await _context.Clinic_Staff.AddAsync(clinic_staff);
				await _context.SaveChangesAsync();
				clinic_Staff_Loggins.Add(new DTO.NetCore.DataObject.LogginModel.Clinic_Staff_Loggin()
				{
					ClinicStaffID = clinic_staff.ClinicStaffID,
					ClinicID = clinic_staff.ClinicID,
					UserID = clinic_staff.UserID,
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert thành công Clinic_Staff!";
				returnData.clinic_Staff_Loggins = clinic_Staff_Loggins;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Insert_Clinic_Staff Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<Response_ClinicStaff_Loggin> Update_Clinic_Staff(Clinic_StaffUpdate update_)
		{
			var returnData = new Response_ClinicStaff_Loggin();
			var clinic_Staff_Loggins = new List<Clinic_Staff_Loggin>();
			try
			{
				var clinic_staff = await _context.Clinic_Staff.FindAsync(update_.ClinicStaffID);
				if (clinic_staff == null) 
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại ClinicStaffID: {update_.ClinicStaffID}. Vui lòng nhập ClinicStaffID khác!";
					return returnData;
				}
				if (await _clinicRepository.GetClinicByID(update_.ClinicID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ClinicID không tồn tại. Vui lòng nhập ClinicID khác!";
					return returnData;
				}
				clinic_staff.ClinicID = update_.ClinicID ?? 0;

				if (await _userRepository.GetUserByUserID(update_.UserID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "UserID không tồn tại. Vui lòng nhập UserID khác!";
					return returnData;
				}
				clinic_staff.UserID = update_.UserID ?? 0;

				_context.Clinic_Staff.Update(clinic_staff);
				await _context.SaveChangesAsync();
				clinic_Staff_Loggins.Add(new DTO.NetCore.DataObject.LogginModel.Clinic_Staff_Loggin()
				{
					ClinicStaffID = clinic_staff.ClinicStaffID,
					ClinicID = clinic_staff.ClinicID,
					UserID = clinic_staff.UserID,
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Update thành công Clinic_Staff: {update_.ClinicStaffID}!";
				returnData.clinic_Staff_Loggins = clinic_Staff_Loggins;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Update_Clinic_Staff Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<Response_ClinicStaff_Loggin> Delete_Clinic_Staff(Clinic_StaffDelete delete_)
		{
			var returnData = new Response_ClinicStaff_Loggin();
			var clinic_Staff_Loggins = new List<Clinic_Staff_Loggin>();
			try
			{
				var clinic_staff = await _context.Clinic_Staff.FindAsync(delete_.ClinicStaffID);
				if (clinic_staff != null) 
				{
					_context.Clinic_Staff.Remove(clinic_staff);
					await _context.SaveChangesAsync();
					clinic_Staff_Loggins.Add(new Clinic_Staff_Loggin
					{
						ClinicStaffID = clinic_staff.ClinicStaffID,
						ClinicID = clinic_staff.ClinicID,
						UserID = clinic_staff.UserID,
					});
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Delete thành công Clinic_Staff: {delete_.ClinicStaffID}!";
					returnData.clinic_Staff_Loggins = clinic_Staff_Loggins;
					return returnData;
				}
				returnData.ResponseCode = -1;
				returnData.ResposeMessage = $"Không tồn tại ClinicStaffID: {delete_.ClinicStaffID}. Vui lòng nhập ClinicStaffID khác!";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Update_Clinic_Staff Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<Response_ClinicStaff_Loggin> GetList_Clinic_Staff(Clinic_StaffGetList getList_)
		{
			var returnData = new Response_ClinicStaff_Loggin();
			var listClinic_Staff = new List<Clinic_StaffResponse>();
			try
			{
				if (getList_.ClinicID != null)
				{
					if (await _clinicRepository.GetClinicByID(getList_.ClinicID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ClinicID không tồn tại. Vui lòng nhập ClinicID khác!";
						return returnData;
					}
				}
				if (getList_.UserID != null)
				{
					if (await _userRepository.GetUserByUserID(getList_.UserID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "UserID không tồn tại. Vui lòng nhập UserID khác!";
						return returnData;
					}
				}
				if (getList_.ClinicStaffID != null)
				{
					if (await GetClinic_StaffByID(getList_.ClinicStaffID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ClinicStaffID không tồn tại. Vui lòng nhập ClinicStaffID khác!";
						return returnData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@ClinicStaffID", getList_.ClinicStaffID ?? null);
				parameters.Add("@ClinicID", getList_.ClinicID ?? null);
				parameters.Add("@UserID", getList_.UserID ?? null);
				var result = await DbConnection.QueryAsync<Clinic_StaffResponse>("GetList_SearchClinicStaff", parameters);
				if (result != null && result.Any())
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Lấy danh sách Clinic_Staff thành công!";
					returnData.Data = result.ToList();
					return returnData;
				}
				returnData.ResponseCode = 0;
				returnData.ResposeMessage = "Không tìm thấy Clinic_Staff nào.";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error GetList_Clinic_Staff Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<Clinic_Staff> GetClinic_StaffByID(int? ClinicStaffID)
		{
			return _context.Clinic_Staff.Where(s => s.ClinicStaffID == ClinicStaffID).FirstOrDefault();
		}
	}
}
