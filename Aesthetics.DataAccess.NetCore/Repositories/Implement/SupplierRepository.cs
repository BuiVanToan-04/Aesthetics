using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
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

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class SupplierRepository : BaseApplicationService, ISupplierRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		public SupplierRepository(DB_Context context,
			IConfiguration configuration, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<Supplier> GetSupplierBySupplierID(int? supplierID)
		{
			return await _context.Supplier.Where(s => s.SupplierID == supplierID
					&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<Supplier> GetSupplierBySupplierName(string? supplierName)
		{
			return await _context.Supplier.Where(s => s.SupplierName == supplierName
					&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<ResponseData> Insert_Supplier(SupplierRequest supplier)
		{
			var returnData = new ResponseData();
			try
			{
				if (!Validation.CheckString(supplier.SupplierName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierName không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(supplier.SupplierName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierName chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (await GetSupplierBySupplierName(supplier.SupplierName) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Supplier: {supplier.SupplierName} đã tồn tại." +
						$"Vui lòng nhập lại SupplierName!";
					return returnData;
				}
				var parameters = new DynamicParameters();
				parameters.Add("@SupplierName", supplier.SupplierName);
				await DbConnection.ExecuteAsync("Insert_Supplier", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Thêm thành công Supplier!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Update_Supplier(Update_Supplier supplier)
		{
			var returnData = new ResponseData();
			try
			{
				if (supplier.SupplierID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierID không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(supplier.SupplierName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierName không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(supplier.SupplierName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierName chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (await GetSupplierBySupplierID(supplier.SupplierID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại Supplier: {supplier.SupplierID}!";
					return returnData;
				}
				if (await GetSupplierBySupplierName(supplier.SupplierName) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Supplier: {supplier.SupplierName} đã tồn tại." +
						$"Vui lòng nhập lại SupplierName!";
					return returnData;
				}
				var parameters = new DynamicParameters();
				parameters.Add("@SupplierID", supplier.SupplierID);
				parameters.Add("@SupplierName", supplier.SupplierName);
				await DbConnection.ExecuteAsync("Update_Supplier", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Cập nhật thông tin thành công!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Delete_Supplier(Delete_Supplier supplier)
		{
			var returnData = new ResponseData();
			try
			{
				if (supplier.SupplierID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierID không hợp lệ!";
					return returnData;
				}
				if (await GetSupplierBySupplierID(supplier.SupplierID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại Supplier: {supplier.SupplierID}!";
					return returnData;
				}
				var supp = await _context.Supplier.FindAsync(supplier.SupplierID);
				if (supp != null)
				{
					supp.DeleteStatus = 0;
					var result = await _context.SaveChangesAsync();
					if (result > 0)
					{
						returnData.ResponseCode = 1;
						returnData.ResposeMessage = $"Xóa Supplier: {supplier.SupplierID} thành công!";
						return returnData;
					}
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

		public async Task<ResponseSupplierData> GetList_SearchSupplier(GetList_SearchSupplier _searchSupplier)
		{
			var responseData = new ResponseSupplierData();
			var responeSupplier = new List<ResponseSupplier>();
			try
			{
				if (_searchSupplier.SupplierID <= 0)
				{
					responseData.ResponseCode = -1;
					responseData.ResposeMessage = "Dữ liệu đầu vào SupplierID không hợp lệ!";
					return responseData;
				}
				if (_searchSupplier.SupplierID != null)
				{
					if (await GetSupplierBySupplierID(_searchSupplier.SupplierID) == null)
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = $"Danh sách không tồn tại Supplier có mã: {_searchSupplier.SupplierID}!";
						return responseData;
					}
				}
				if (_searchSupplier.SupplierName != null)
				{
					if (!Validation.CheckXSSInput(_searchSupplier.SupplierName))
					{
						responseData.ResponseCode = -1;
						responseData.ResposeMessage = "SupplierName chứa kí tự không hợp lệ!";
						return responseData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@SupplierID", _searchSupplier.SupplierID ?? null);
				parameters.Add("@SupplierName", _searchSupplier.SupplierName ?? null);
				var result = await DbConnection.QueryAsync<ResponseSupplier>("GetList_SearchSupplier", parameters);
				if (result != null && result.Any())
				{
					responseData.ResponseCode = 1;
					responseData.ResposeMessage = "Lấy danh sách Supplier thành công!";
					responseData.Data = result.ToList();
					return responseData;
				}
				else
				{
					responseData.ResponseCode = 0;
					responseData.ResposeMessage = "Không tìm thấy Supplier nào.";
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
