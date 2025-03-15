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
				var newProOfSer = new TypeProductsOfServices
				{
					ProductsOfServicesName = request.ProductsOfServicesName,
					ProductsOfServicesType = request.ProductsOfServicesType,
					DeleteStatus = 1
				};
				await _context.TypeProductsOfServices.AddAsync(newProOfSer);
				await _context.SaveChangesAsync();
				productOfServicess_Loggin.Add(new ProductsOfServices_Logginn
				{
					ProductsOfServicesID = newProOfSer.ProductsOfServicesID,
					ProductsOfServicesName = newProOfSer.ProductsOfServicesName,
					ProductsOfServicesType = newProOfSer.ProductsOfServicesType,
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
				var _productOfServicess = await GetTypeProductsOfServicesIDByID(update_.ProductsOfServicesID);
				if (update_.ProductsOfServicesID <= 0 || _productOfServicess == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ || Không tồn tại!";
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

				if (await GetTypeByName(update_.ProductsOfServicesName, update_.ProductsOfServicesType) != null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"{update_.ProductsOfServicesName} " +
						$"đã tồn tại trong {update_.ProductsOfServicesType}." +
						$" Vui lòng nhập tên khác hoặc kiểu khác!";
					return returnData;
				}
				_productOfServicess.ProductsOfServicesName = update_.ProductsOfServicesName ?? null;
				_productOfServicess.ProductsOfServicesType = update_.ProductsOfServicesType ?? null;
				_productOfServicess.DeleteStatus = 1;

				_context.TypeProductsOfServices.Update(_productOfServicess);
				await _context.SaveChangesAsync();

				productOfServicess_Loggin.Add(new ProductsOfServices_Logginn
				{
					ProductsOfServicesID = _productOfServicess.ProductsOfServicesID,
					ProductsOfServicesName = _productOfServicess.ProductsOfServicesName ?? null,
					ProductsOfServicesType = _productOfServicess.ProductsOfServicesType ?? null,
					DeleteStatus = 1
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = $"Update thành công ProductsOfServices";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Update_TypeProductsOfServices Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ProductsOfServices_LogginDelete> Delete_TypeProductsOfServices(Delete_TypeProductsOfServices delete_)
		{
			var returnData = new ProductsOfServices_LogginDelete();
			var productOfServicess_Loggin = new List<ProductsOfServices_Logginn>();
			var clinic_Loggins = new List<Clinic_Loggin>();
			var servicess_Loggins = new List<Servicess_Loggin>();
			var products_Loggins = new List<Products_Loggin>();
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				if (delete_.ProductsOfServicesID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
					return returnData;
				}
				var productsServices = await _context.TypeProductsOfServices
					.Include(p => p.Products)
					.Include(s => s.Services)
					.Include(c => c.Clinic)
					.AsSplitQuery()
					.FirstOrDefaultAsync(s => s.ProductsOfServicesID == delete_.ProductsOfServicesID);
				if (productsServices != null)
				{
					//1. Xóa ProductOfServicess
					productsServices.DeleteStatus = 0;
					productOfServicess_Loggin.Add(new ProductsOfServices_Logginn
					{
						ProductsOfServicesID = delete_.ProductsOfServicesID,
						ProductsOfServicesName = productsServices.ProductsOfServicesName,
						ProductsOfServicesType = productsServices.ProductsOfServicesType,
						DeleteStatus = 0
					});

					//2. Xóa Producst liên quan đến ProductsOfServicesID
					var products = productsServices.Products
							.Where(s => s.ProductsOfServicesID == productsServices.ProductsOfServicesID).ToList();
					if (products != null && products.Any())
					{
						foreach (var pro in products)
						{
							pro.DeleteStatus = 0;
							products_Loggins.Add(new Products_Loggin
							{
								ProductID = pro.ProductID,
								ProductName = pro.ProductName,
								ProductsOfServicesID = pro.ProductsOfServicesID,
								SupplierID = pro.SupplierID,
								ProductDescription = pro.ProductDescription,
								SellingPrice = pro.SellingPrice,
								Quantity = pro.Quantity,
								ProductImages = pro.ProductImages,
								DeleteStatus = 0
							});
						}
					}

					//3. Xóa Services liên quan đến ProductsOfServicesID
					var services = productsServices.Services
							.Where(s => s.ProductsOfServicesID == productsServices.ProductsOfServicesID).ToList();
					if (services != null && services.Any())
					{
						foreach (var ser in services)
						{
							ser.DeleteStatus = 0;
							servicess_Loggins.Add(new Servicess_Loggin
							{
								ServiceID = ser.ServiceID,
								ProductsOfServicesID = ser.ProductsOfServicesID,
								ServiceName = ser.ServiceName,
								Description = ser.Description,
								ServiceImage = ser.ServiceImage,
								PriceService = ser.PriceService,
								DeleteStatus = 0
							});
						}
					}

					//4. Xóa Clinic liên quan đến ProductsOfServicesID
					var clinics = productsServices.Clinic;
					if (clinics != null && clinics.ProductsOfServicesID == productsServices.ProductsOfServicesID)
					{
						clinics.ClinicStatus = 0;
						clinic_Loggins.Add(new Clinic_Loggin
						{
							ClinicID = clinics.ClinicID,
							ClinicName = clinics.ClinicName,
							ProductsOfServicesID = clinics.ProductsOfServicesID,
							ProductsOfServicesName = clinics.ProductsOfServicesName,
							ClinicStatus = 0
						});
					}
					//Commit transaction nếu thành công
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Xóa thành công ProductOfServices: {delete_.ProductsOfServicesID}";
					returnData.productOfServicess_Loggin = productOfServicess_Loggin;
					returnData.products_Loggins = products_Loggins;
					returnData.servicess_Loggins = servicess_Loggins;
					returnData.clinic_Loggins = clinic_Loggins;
					return returnData;
				}
				returnData.ResponseCode = 0;
				returnData.ResposeMessage = $"Services ID: {delete_.ProductsOfServicesID} không tồn tại";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Delete_TypeProductsOfServices Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ProductsOfServices_Loggin> GetList_SearchTypeProductsOfServices(GetList_SearchTypeProductsOfServices getList_Search)
		{
			var returnData = new ProductsOfServices_Loggin();
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
