using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public async Task<ResponseSupplier_Loggin> Insert_Supplier(SupplierRequest supplier)
		{
			var returnData = new ResponseSupplier_Loggin();
			var supplier_Loggins = new List<Supplier_Loggin>();
			try
			{
				if (!Validation.CheckXSSInput(supplier.SupplierName) || !Validation.CheckString(supplier.SupplierName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierName không hợp lệ || Dữ liệu đầu vào SupplierName chứa kí tự không hợp lệ!";
					return returnData;
				}
				
				if (await GetSupplierBySupplierName(supplier.SupplierName) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Supplier: {supplier.SupplierName} đã tồn tại." +
						$"Vui lòng nhập lại SupplierName!";
					return returnData;
				}
				var newSupplier = new Supplier
				{
					SupplierName = supplier.SupplierName,
					DeleteStatus = 1
				};
				await _context.Supplier.AddAsync(newSupplier);
				await _context.SaveChangesAsync();
				supplier_Loggins.Add(new Supplier_Loggin
				{
					SupplierID = newSupplier.SupplierID,
					SupplierName = newSupplier.SupplierName,
					DeleteStatus = 1
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Thêm thành công Supplier!";
				returnData.supplier_Loggins = supplier_Loggins;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Insert_Supplier Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseSupplier_Loggin> Update_Supplier(Update_Supplier supplier)
		{
			var returnData = new ResponseSupplier_Loggin();
			var supplier_Loggins = new List<Supplier_Loggin>();
			try
			{
				var _supplier = await GetSupplierBySupplierID(supplier.SupplierID);
				if (supplier.SupplierID <= 0 || _supplier == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierID không hợp lệ || không tồn tại!";
					return returnData;
				}
				if (!Validation.CheckString(supplier.SupplierName) || !Validation.CheckXSSInput(supplier.SupplierName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierName không hợp lệ || SupplierName chứa kí tự không hợp lệ!";
					return returnData;
				}
			
				if (await GetSupplierBySupplierName(supplier.SupplierName) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Supplier: {supplier.SupplierName} đã tồn tại." +
						$"Vui lòng nhập lại SupplierName!";
					return returnData;
				}
				_supplier.SupplierID = supplier.SupplierID;
				_supplier.SupplierName = supplier.SupplierName;
				_supplier.DeleteStatus = 1;
				_context.Supplier.Update(_supplier);
				await _context.SaveChangesAsync();
				supplier_Loggins.Add(new Supplier_Loggin
				{
					SupplierID = _supplier.SupplierID,
					SupplierName = _supplier.SupplierName,
					DeleteStatus = 1
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Cập nhật thông tin thành công!";
				returnData.supplier_Loggins = supplier_Loggins;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Update_Supplier Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseSupplier_Loggin> Delete_Supplier(Delete_Supplier supplier)
		{
			var returnData = new ResponseSupplier_Loggin();
			var supplier_Loggins = new List<Supplier_Loggin>();
			try
			{
				if (supplier.SupplierID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào SupplierID không hợp lệ!";
					return returnData;
				}
				var supp = await _context.Supplier.FindAsync(supplier.SupplierID);
				if (supp != null)
				{
					supp.DeleteStatus = 0;
					await _context.SaveChangesAsync();
					supplier_Loggins.Add(new Supplier_Loggin
					{
						SupplierID = supplier.SupplierID,
						SupplierName = supp.SupplierName,
						DeleteStatus = 0
					});
					returnData.ResponseCode = 1;
					returnData.supplier_Loggins = supplier_Loggins;
					returnData.ResposeMessage = $"Xóa Supplier: {supplier.SupplierID} thành công!";
					return returnData;
				}
				returnData.ResponseCode = 0;
				returnData.ResposeMessage = $"Không tìm thấy Supplier: {supplier.SupplierID}!";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Delete_Supplier Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseSupplier_Loggin> GetList_SearchSupplier(GetList_SearchSupplier _searchSupplier)
		{
			var responseData = new ResponseSupplier_Loggin();
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
				responseData.ResponseCode = 0;
				responseData.ResposeMessage = "Không tìm thấy Supplier nào.";
				return responseData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error GetList_SearchSupplier Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
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
	}
}
