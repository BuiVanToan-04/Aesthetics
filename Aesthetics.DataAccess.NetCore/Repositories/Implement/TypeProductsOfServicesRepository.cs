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

		public async Task<TypeProductsOfServices> GetTypeProductsOfServicesByName(string? ProductsOfServicesName)
		{
			return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesName == ProductsOfServicesName
				&& s.DeleteStatus == 1)
				.FirstOrDefaultAsync();
		}

		public async Task<TypeProductsOfServices> GetTypeByName(string? Name, string? Type)
		{
			return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesName == Name
							&& s.ProductsOfServicesType == Type
							&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<TypeProductsOfServices> GetTypeProductsOfServicesIDByID(int? ProductsOfServicesID)
		{
			return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesID == ProductsOfServicesID
			&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<ProductsOfServices_Loggin> Insert_TypeProductsOfServices(TypeProductsOfServicesRequest request)
		{
			var returnData = new ProductsOfServices_Loggin();
			var productOfServicess_Loggin = new List<ProductsOfServices_Logginn>();
			try
			{
				if (!Validation.CheckString(request.ProductsOfServicesName) || !Validation.CheckXSSInput(request.ProductsOfServicesName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesName không hợp lệ || ProductsOfServicesName chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(request.ProductsOfServicesType) || !Validation.CheckXSSInput(request.ProductsOfServicesType))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesType không hợp lệ || ProductsOfServicesType chứa kí tự không hợp lệ!";
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
				parameters.Add("@ProductsOfServicesID", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
				await DbConnection.ExecuteAsync("Insert_TypeProductsOfServices", parameters);
				var newProductsOfServicesID = parameters.Get<int>("@ProductsOfServicesID");
				productOfServicess_Loggin.Add(new ProductsOfServices_Logginn
				{
					ProductsOfServicesID = newProductsOfServicesID,
					ProductsOfServicesName = request.ProductsOfServicesName,
					ProductsOfServicesType = request.ProductsOfServicesType,
					DeleteStatus = 1
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Thêm thành công Name:{request.ProductsOfServicesName}, Type: {request.ProductsOfServicesType}";
				returnData.productOfServicess_Loggin = productOfServicess_Loggin;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Insert_TypeProductsOfServices Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ProductsOfServices_Loggin> Update_TypeProductsOfServices(Update_TypeProductsOfServices update_)
		{
			var returnData = new ProductsOfServices_Loggin();
			var productOfServicess_Loggin = new List<ProductsOfServices_Logginn>();
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
				productOfServicess_Loggin.Add(new ProductsOfServices_Logginn
				{
					ProductsOfServicesID = update_.ProductsOfServicesID,
					ProductsOfServicesName = update_.ProductsOfServicesName ?? null,
					ProductsOfServicesType = update_.ProductsOfServicesType ?? null,
					DeleteStatus = 1
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Update thành công Name:{update_.ProductsOfServicesName}, Type: {update_.ProductsOfServicesType}";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Update_TypeProductsOfServices Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ProductsOfServices_Loggin> Delete_TypeProductsOfServices(Delete_TypeProductsOfServices delete_)
		{
			var returnData = new ProductsOfServices_Loggin();
			var productOfServicess_Loggin = new List<ProductsOfServices_Logginn>();
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
					productOfServicess_Loggin.Add(new ProductsOfServices_Logginn
					{
						ProductsOfServicesID = delete_.ProductsOfServicesID,
						ProductsOfServicesName = productsServices.ProductsOfServicesName,
						ProductsOfServicesType = productsServices.ProductsOfServicesType,
						DeleteStatus = 0
					});
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Xóa thành công Product || Services ID: {delete_.ProductsOfServicesID}";
					return returnData;
				}
				returnData.ResponseCode = 0;
				returnData.ResposeMessage = $"Services ID: {delete_.ProductsOfServicesID}";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Delete_TypeProductsOfServices Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
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
					("GetList_SearchProductsOfServices", parameters);
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
				throw new Exception($"Error GetList_SearchTypeProductsOfServices Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}
	}
}
