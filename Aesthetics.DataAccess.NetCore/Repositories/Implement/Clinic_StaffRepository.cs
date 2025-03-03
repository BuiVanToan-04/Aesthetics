using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.Dapper;
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
		public async Task<ResponseData> Insert_Clinic_Staff(Clinic_StaffRequest insert_)
		{
			var returnData = new ResponseData();
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
					DeleteStatus = 1,
				};
				await _context.Clinic_Staff.AddAsync(clinic_staff);
				await _context.SaveChangesAsync();
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert thành công Clinic_Staff!";
				return returnData;
			}
			catch(Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Update_Clinic_Staff(Clinic_StaffUpdate update_)
		{
			var returnData = new ResponseData();
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
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Update thành công Clinic_Staff: {update_.ClinicStaffID}!";
				return returnData;
			}
			catch(Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Delete_Clinic_Staff(Clinic_StaffDelete delete_)
		{
			var returnData = new ResponseData();
			try
			{
				var clinic_staff = await _context.Clinic_Staff.FindAsync(delete_.ClinicStaffID);
				if (clinic_staff != null) 
				{
					clinic_staff.DeleteStatus = 0;
					await _context.SaveChangesAsync();
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Delete thành công Clinic_Staff: {delete_.ClinicStaffID}!";
					return returnData;
				}
				returnData.ResponseCode = -1;
				returnData.ResposeMessage = $"Không tồn tại ClinicStaffID: {delete_.ClinicStaffID}. Vui lòng nhập ClinicStaffID khác!";
				return returnData;
			}
			catch(Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<Clinic_StaffData> GetList_Clinic_Staff(Clinic_StaffGetList getList_)
		{
			throw new NotImplementedException();
		}
	}
}
