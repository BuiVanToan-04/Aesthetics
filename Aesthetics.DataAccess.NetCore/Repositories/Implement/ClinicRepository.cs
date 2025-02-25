using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class ClinicRepository : BaseApplicationService, IClinicRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		private ITypeProductsOfServicesRepository _productsOfServicesRepository;
		public ClinicRepository(DB_Context context, IConfiguration configuration,
			IServiceProvider serviceProvider,
			ITypeProductsOfServicesRepository productsOfServicesRepository) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_productsOfServicesRepository = productsOfServicesRepository;
		}

		public async Task<Clinic> GetClinicByName(string? name)
		{
			return await _context.Clinic.Where(s => s.ClinicName == name && s.ClinicStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<Clinic> GetClinicByID(int? ClinicID)
		{
			return await _context.Clinic.Where(s => s.ClinicID == ClinicID && s.ClinicStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<ResponseData> Insert_Clinic(ClinicRequest insert_)
		{
			var returnData = new ResponseData();
			try
			{
				if (!Validation.CheckString(insert_.ClinicName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu đầu vào {insert_.ClinicName} không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(insert_.ClinicName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu {insert_.ClinicName} chứa kí tự không hợp lệ!";
					return returnData;
				}

				if (await GetClinicByName(insert_.ClinicName) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"{insert_.ClinicName} đã tồn tại. Vui lòng nhập ClinicName khác!";
					return returnData;
				}

				if (insert_.ProductsOfServicesID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu đầu vào {insert_.ProductsOfServicesID} không hợp lệ!";
					return returnData;
				}
				if (await _productsOfServicesRepository
					.GetTypeProductsOfServicesIDByID(insert_.ProductsOfServicesID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại ProductsOfServicesID: {insert_.ProductsOfServicesID}";
					return returnData;
				}
				if (!Validation.CheckString(insert_.ProductsOfServicesName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu đầu vào {insert_.ProductsOfServicesName} không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(insert_.ProductsOfServicesName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Dữ liệu {insert_.ProductsOfServicesName} chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (await _productsOfServicesRepository
					.GetTypeProductsOfServicesByName(insert_.ProductsOfServicesName) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại ProductsOfServices có Name: {insert_.ProductsOfServicesName}";
					return returnData;
				}

				var parameters = new DynamicParameters();
				parameters.Add("@ClinicName", insert_.ClinicName);
				parameters.Add("@ProductsOfServicesID", insert_.ProductsOfServicesID);
				parameters.Add("@ProductsOfServicesName", insert_.ProductsOfServicesName);
				await DbConnection.ExecuteAsync("Insert_Clinic", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert thành công Clinic!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Update_Clinic(Update_Clinic update_)
		{
			var returnData = new ResponseData();
			try
			{
				if (update_.ClinicID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ClinicID không hợp lệ!";
					return returnData;
				}
				if (await GetClinicByID(update_.ClinicID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại Clinic có ID: {update_.ClinicID}!";
					return returnData;
				}
				if (update_.ClinicName != null)
				{
					if (!Validation.CheckString(update_.ClinicName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Dữ liệu đầu vào {update_.ClinicName} không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.ClinicName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Dữ liệu {update_.ClinicName} chứa kí tự không hợp lệ!";
						return returnData;
					}

					if (await GetClinicByName(update_.ClinicName) != null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"{update_.ClinicName} đã tồn tại. Vui lòng nhập ClinicName khác!";
						return returnData;
					}
				}
				if (update_.ProductsOfServicesID != null)
				{
					if (update_.ProductsOfServicesID <= 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Dữ liệu đầu vào {update_.ProductsOfServicesID} không hợp lệ!";
						return returnData;
					}
					if (await _productsOfServicesRepository
						.GetTypeProductsOfServicesIDByID(update_.ProductsOfServicesID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Không tồn tại ProductsOfServicesID: {update_.ProductsOfServicesID}";
						return returnData;
					}
				}
				if (update_.ProductsOfServicesName != null)
				{
					if (!Validation.CheckString(update_.ProductsOfServicesName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Dữ liệu đầu vào {update_.ProductsOfServicesName} không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.ProductsOfServicesName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Dữ liệu {update_.ProductsOfServicesName} chứa kí tự không hợp lệ!";
						return returnData;
					}
					if (await _productsOfServicesRepository
						.GetTypeProductsOfServicesByName(update_.ProductsOfServicesName) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Không tồn tại ProductsOfServices có Name: {update_.ProductsOfServicesName}";
						return returnData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@ClinicID", update_.ClinicID);
				parameters.Add("@ClinicName", update_.ClinicName ?? null);
				parameters.Add("@ProductsOfServicesID", update_.ProductsOfServicesID ?? null);
				parameters.Add("@ProductsOfServicesName", update_.ProductsOfServicesName ?? null);
				await DbConnection.ExecuteAsync("Update_Clinic", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Update thành công Clinic!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Delete_Clinic(Delete_Clinic delete_)
		{
			var returnData = new ResponseData();
			try
			{
				if (delete_.ClinicID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ClinicID không hợp lệ!";
					return returnData;
				}
				if (await GetClinicByID(delete_.ClinicID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"ClinicID: {delete_.ClinicID} không tồn tại!";
					return returnData;
				}
				var Clinic = await _context.Clinic.FindAsync(delete_.ClinicID);
				if (Clinic != null)
				{
					Clinic.ClinicStatus = 0;
					await _context.SaveChangesAsync();
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Delete thành công!";
					return returnData;
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

		public async Task<ResponesClinicData> GetList_SearchClinic(GetList_Search getList_)
		{
			var responseData = new ResponesClinicData();
			var listClinic = new List<ResponesClinic>();
			try
			{
				if (getList_.ClinicID != null)
				{
					if (getList_.ClinicID <= 0)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "Dữ liệu đầu vào ClinicID không hợp lệ!";
						return responseData;
					}
					if (await GetClinicByID(getList_.ClinicID) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Không tồn tại Clinic có ID: {getList_.ClinicID}";
						return responseData;
					}
				}
				if (getList_.ClinicName != null)
				{
					if (!Validation.CheckString(getList_.ClinicName))
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Dữ liệu đầu vào {getList_.ClinicName} không hợp lệ!";
						return responseData;
					}
					if (!Validation.CheckXSSInput(getList_.ClinicName))
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Dữ liệu {getList_.ClinicName} chứa kí tự không hợp lệ!";
						return responseData;
					}

					if (await GetClinicByName(getList_.ClinicName) != null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"{getList_.ClinicName} đã tồn tại. Vui lòng nhập ClinicName khác!";
						return responseData;
					}
				}
				if (getList_.ProductsOfServicesID != null)
				{
					if (getList_.ProductsOfServicesID <= 0)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
						return responseData;
					}
					if (await _productsOfServicesRepository
					.GetTypeProductsOfServicesIDByID(getList_.ProductsOfServicesID) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Không tồn tại Clinic có ProductsOfServices: {getList_.ProductsOfServicesID}";
						return responseData;
					}
				}
				if (getList_.ProductsOfServicesName != null)
				{
					if (!Validation.CheckString(getList_.ProductsOfServicesName))
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Dữ liệu đầu vào {getList_.ProductsOfServicesName} không hợp lệ!";
						return responseData;
					}
					if (!Validation.CheckXSSInput(getList_.ProductsOfServicesName))
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Dữ liệu {getList_.ProductsOfServicesName} chứa kí tự không hợp lệ!";
						return responseData;
					}
					if (await _productsOfServicesRepository
						.GetTypeProductsOfServicesByName(getList_.ProductsOfServicesName) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Không tồn tại ProductsOfServices có Name: {getList_.ProductsOfServicesName}";
						return responseData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@ClinicID",getList_.ClinicID ?? null);
				parameters.Add("@ClinicName", getList_.ClinicName ?? null);
				parameters.Add("@ProductsOfServicesID", getList_.ProductsOfServicesID ?? null);
				parameters.Add("@ProductsOfServicesName", getList_.ProductsOfServicesName ?? null);
				var result = await DbConnection.QueryAsync<ResponesClinic>("GetList_SearchLinic", parameters);
				if (result != null && result.Any())
				{
					responseData.ResponseCode = 1;
					responseData.ResposeMessage = "Lấy thành công danh sách";
					responseData.Data = result.ToList();
					return responseData;
				}
				else
				{
					responseData.ResponseCode = 0;
					responseData.ResposeMessage = "Không tìm thấy Clinic nào.";
					return responseData;
				}
			}
			catch (Exception ex)
			{
				responseData.ResponseCode = -99;
				responseData.ResposeMessage = ex.Message;
				return responseData;
			}
		}
	}

}
