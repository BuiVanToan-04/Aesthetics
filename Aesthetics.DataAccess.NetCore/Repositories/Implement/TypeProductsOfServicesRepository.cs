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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class TypeProductsOfServicesRepository : BaseApplicationService, ITypeProductsOfServicesRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		public TypeProductsOfServicesRepository(DB_Context context, IConfiguration configuration,
			IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<TypeProductsOfServices> GetTypeByName(string? Name, string? Type)
		{
			//return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesName == Name
			//		&& s.DeleteStatus == 1)
			//	.OrderByDescending(s => _context.TypeProductsOfServices
			//	.Any(t => t.ProductsOfServicesName == Name && t.ProductsOfServicesType == s.ProductsOfServicesType))
			//	.FirstOrDefaultAsync();
			return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesName == Name
							&& s.ProductsOfServicesType == Type).FirstOrDefaultAsync();
		}

		public async Task<TypeProductsOfServices> GetTypeProductsOfServicesIDByID(int? ProductsOfServicesID)
		{
			return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesID == ProductsOfServicesID).FirstOrDefaultAsync();
		}

		public async Task<ResponseData> Insert_TypeProductsOfServices(TypeProductsOfServicesRequest request)
		{
			var returnData = new ResponseData();
			try
			{
				if (!Validation.CheckString(request.ProductsOfServicesName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesName không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(request.ProductsOfServicesName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ProductsOfServicesName chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(request.ProductsOfServicesType))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesName không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(request.ProductsOfServicesType))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ProductsOfServicesName chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (await GetTypeByName(request.ProductsOfServicesName, request.ProductsOfServicesType) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"{request.ProductsOfServicesName} " +
						$"đã tồn tại trong {request.ProductsOfServicesType}." +
						$" Vui lòng nhập tên khác hoặc kiểu khác!";
					return returnData;
				}
				var parameters = new DynamicParameters();
				parameters.Add("@ProductsOfServicesName", request.ProductsOfServicesName ?? null);
				parameters.Add("@ProductsOfServicesType", request.ProductsOfServicesType ?? null);
				await DbConnection.ExecuteAsync("Insert_TypeProductsOfServices", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Thêm thành công Name:{request.ProductsOfServicesName}, Type: {request.ProductsOfServicesType}";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Update_TypeProductsOfServices(Update_TypeProductsOfServices update_)
		{
			var returnData = new ResponseData();
			try
			{
				if (update_.ProductsOfServicesID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
					return returnData;
				}
				if(update_.ProductsOfServicesName != null)
				{
					if (!Validation.CheckString(update_.ProductsOfServicesName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesName không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.ProductsOfServicesName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ProductsOfServicesName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (update_.ProductsOfServicesType != null)
				{
					if (!Validation.CheckString(update_.ProductsOfServicesType))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesName không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.ProductsOfServicesType))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ProductsOfServicesName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}

				if (await GetTypeProductsOfServicesIDByID(update_.ProductsOfServicesID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại loại Product || Services có ID: {update_.ProductsOfServicesID}!";
					return returnData;
				}

				if (await GetTypeByName(update_.ProductsOfServicesName, update_.ProductsOfServicesType) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"{update_.ProductsOfServicesName} " +
						$"đã tồn tại trong {update_.ProductsOfServicesType}." +
						$" Vui lòng nhập tên khác hoặc kiểu khác!";
					return returnData;
				}
				var parameters = new DynamicParameters();
				parameters.Add("@ProductsOfServicesID", update_.ProductsOfServicesID);
				parameters.Add("@ProductsOfServicesName", update_.ProductsOfServicesName ?? null);
				parameters.Add("@ProductsOfServicesType", update_.ProductsOfServicesType ?? null);
				await DbConnection.ExecuteAsync("Update_TypeProductsOfServices", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Update thành công Name:{update_.ProductsOfServicesName}, Type: {update_.ProductsOfServicesType}";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Delete_TypeProductsOfServices(Delete_TypeProductsOfServices delete_)
		{
			var returnData = new ResponseData();
			try
			{
				if (delete_.ProductsOfServicesID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
					return returnData;
				}
				if (await GetTypeProductsOfServicesIDByID(delete_.ProductsOfServicesID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại Sản Phẩm || Dịch vụ có ID: {delete_.ProductsOfServicesID}!";
					return returnData;
				}
				var productsServices = await _context.TypeProductsOfServices.FindAsync(delete_.ProductsOfServicesID);
				if (productsServices != null)
				{
					productsServices.DeleteStatus = 0;
					await _context.SaveChangesAsync();
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Xóa thành công Product || Services ID: {delete_.ProductsOfServicesID}";
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

		public async Task<ProductsOfServicesData> GetList_SearchTypeProductsOfServices(GetList_SearchTypeProductsOfServices getList_Search)
		{
			var returnData = new ProductsOfServicesData();
			var listProductsServices = new List<ProductsOfServicesRespones>();
			try
			{
				if (getList_Search.ProductsOfServicesID != null)
				{
					if (getList_Search.ProductsOfServicesID <= 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
						return returnData;
					}
					if (await GetTypeProductsOfServicesIDByID(getList_Search.ProductsOfServicesID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Không tồn tại loại Product || Services có ID: {getList_Search.ProductsOfServicesID}!";
						return returnData;
					}
				}
				if (getList_Search.ProductsOfServicesName != null) 
				{
					if (!Validation.CheckString(getList_Search.ProductsOfServicesName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesName không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(getList_Search.ProductsOfServicesName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ProductsOfServicesName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (getList_Search.ProductsOfServicesType != null)
				{
					if (!Validation.CheckString(getList_Search.ProductsOfServicesType))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesName không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(getList_Search.ProductsOfServicesType))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ProductsOfServicesName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				var parameters = new DynamicParameters();
				parameters.Add("@ProductsOfServicesID",getList_Search.ProductsOfServicesID ?? null);
				parameters.Add("@ProductsOfServicesName", getList_Search.ProductsOfServicesName ?? null);
				parameters.Add("@ProductsOfServicesType", getList_Search.ProductsOfServicesType ?? null);
				var result = await DbConnection.QueryAsync<ProductsOfServicesRespones>
					("GetList&SearchTypeProductsOfServices", parameters);
				if(result != null && result.Any())
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Lấy thành công List ProductsOfServices!";
					returnData.Data = result.ToList();
					return returnData;
				}
				else
				{
					returnData.ResponseCode = 0;
					returnData.ResposeMessage = "Không lấy đc ProductsOfServices nào!";
					return returnData;
				}
			}
			catch (Exception ex) 
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage= ex.Message;
				return returnData;
			}
		}

		public async Task<TypeProductsOfServices> GetTypeProductsOfServicesByName(string? ProductsOfServicesName)
		{
			return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesName == ProductsOfServicesName)
				.FirstOrDefaultAsync();
		}
	}
}
