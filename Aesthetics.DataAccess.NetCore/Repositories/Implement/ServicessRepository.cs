using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using BE_102024.DataAces.NetCore.CheckConditions;
using BE_102024.DataAces.NetCore.Dapper;
using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
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
	public class ServicessRepository : BaseApplicationService, IServicessRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		private ITypeProductsOfServicesRepository _typeProductsOfServices;
		private static List<Servicess> _listSevicess;
		public ServicessRepository(DB_Context context, IConfiguration configuration,
			ITypeProductsOfServicesRepository typeProductsOfServices, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_typeProductsOfServices = typeProductsOfServices;
			_listSevicess = new List<Servicess>();
		}

		public async Task<string> BaseProcessingFunction64(string? ServicessImage)
		{
			try
			{
				var path = "FilesImages/Servicess";

				if (!System.IO.Directory.Exists(path))
				{
					System.IO.Directory.CreateDirectory(path);
				}
				string imageName = Guid.NewGuid().ToString() + ".png";
				var imgPath = Path.Combine(path, imageName);

				if (ServicessImage.Contains("data:image"))
				{
					ServicessImage = ServicessImage.Substring(ServicessImage.LastIndexOf(',') + 1);
				}

				byte[] imageBytes = Convert.FromBase64String(ServicessImage);
				MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
				ms.Write(imageBytes, 0, imageBytes.Length);
				System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);

				image.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png);

				return imageName;
			}
			catch (Exception ex)
			{
				throw new Exception("Lỗi khi lưu file: " + ex.Message);
			}
		}

		public async Task<Servicess> GetServicessByServicesID(int? ServicesID)
		{
			return await _context.Servicess.Where(s => s.ServiceID == ServicesID && s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		//public async Task<TypeProductsOfServices> GetTypeProductsOfServices(int? TypeProductsOfServicesID, string? ProductsOfServicesType)
		//{
		//	return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesID == TypeProductsOfServicesID
		//			&& s.ProductsOfServicesType == ProductsOfServicesType
		//			&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		//}

		public async Task<TypeProductsOfServices> GetTypeProductsOfServicesByName(string? ProductsOfServicesType)
		{
			return await _context.TypeProductsOfServices.Where(s => s.DeleteStatus == 1 
					&& s.ProductsOfServicesType == ProductsOfServicesType).FirstOrDefaultAsync();
		}

		public async Task<ResponseData> Insert_Servicess(ServicessRequest servicess_)
		{
			var returnData = new ResponseData();
			try
			{
				if (servicess_.ProductsOfServicesID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(servicess_.ServiceName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ServiceName không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(servicess_.ServiceName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ServiceName chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(servicess_.Description))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào Description không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(servicess_.Description))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu Description chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (servicess_.PriceService < 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào PriceService không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckXSSInput(servicess_.ServiceImage))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu ServiceImage chứa kí tự không hợp lệ!";
					return returnData;
				}
				if (!Validation.CheckString(servicess_.ServiceImage))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ServiceImage không hợp lệ!";
					return returnData;
				}
				

				var imagePathServicess = await BaseProcessingFunction64(servicess_.ServiceImage);
				var parameters = new DynamicParameters();
				parameters.Add("@ProductsOfServicesID",servicess_.ProductsOfServicesID);
				parameters.Add("@ServiceName", servicess_.ServiceName);
				parameters.Add("@Description", servicess_.Description);
				parameters.Add("@ServiceImage", imagePathServicess);
				parameters.Add("@PriceService", servicess_.PriceService);
				await DbConnection.ExecuteAsync("Insert_Servicess", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Insert thành công Service!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Update_Servicess(Update_Servicess update_)
		{
			var returnData = new ResponseData();
			try
			{
				if (update_.ServiceID <= 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ServiceID không hợp lệ!";
					return returnData;
				}
				if (update_.ProductsOfServicesID != null)
				{
					if (update_.ProductsOfServicesID <= 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
						return returnData;
					}
					if (await _typeProductsOfServices.GetTypeProductsOfServicesIDByID(update_.ProductsOfServicesID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductsOfServicesID không tồn tại. Vui lòng nhập lại!";
						return returnData;
					}
				}
	
				if (update_.ServiceName != null) 
				{
					if (!Validation.CheckString(update_.ServiceName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ServiceName không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.ServiceName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ServiceName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (update_.Description != null) 
				{
					if (!Validation.CheckString(update_.Description))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào Description không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(update_.Description))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu Description chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (update_.PriceService != null)
				{
					if (update_.PriceService < 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào PriceService không hợp lệ!";
						return returnData;
					}
				}
				if (update_.ServiceImage != null)
				{
					if (!Validation.CheckXSSInput(update_.ServiceImage))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ServiceImage chứa kí tự không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckString(update_.ServiceImage))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ServiceImage không hợp lệ!";
						return returnData;
					}
					
				}
				var imagePathServicess = update_.ServiceImage != null 
					? await BaseProcessingFunction64(update_.ServiceImage) : null;
				var parameters = new DynamicParameters();
				parameters.Add("@ServiceID", update_.ServiceID);
				parameters.Add("@ProductsOfServicesID", update_.ProductsOfServicesID ?? null);
				parameters.Add("@ServiceName", update_.ServiceName ?? null);
				parameters.Add("@Description", update_.Description ?? null);
				parameters.Add("@ServiceImage",imagePathServicess ?? null);
				parameters.Add("@PriceService", update_.PriceService ?? null);
				await DbConnection.ExecuteAsync("Update_Servicess", parameters);
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Update thành công Service!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> Delete_Servicess(Delete_Servicess delete_)
		{
			var returnData = new ResponseData();
			try
			{
				if(delete_.ServiceID <=0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Dữ liệu đầu vào ServiceID không hợp lệ!";
					return returnData;
				}
				if (await GetServicessByServicesID(delete_.ServiceID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = $"Không tồn tại Service có ID: {delete_.ServiceID}";
					return returnData;
				}
				var service = await _context.Servicess.FindAsync(delete_.ServiceID);
				if (service != null)
				{
					service.DeleteStatus = 0;
					await _context.SaveChangesAsync();
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = $"Delete thành công Service có ID: {delete_.ServiceID}";
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

		public async Task<ResponseServicessData> GetList_SearchServicess(GetList_SearchServicess getList_)
		{
			var returnData = new ResponseServicessData();
			var listData = new List<ResponseServicess>();
			try
			{
				if (getList_.ServiceID != null)
				{
					if (getList_.ServiceID <=0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ServiceID không hợp lệ!";
						return returnData;
					}
					if (await GetServicessByServicesID(getList_.ServiceID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Danh sách không tồn tại Service: {getList_.ServiceID}!";
						return returnData;
					}
				}
				if (getList_.ServiceName != null)
				{
					if (!Validation.CheckString(getList_.ServiceName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ServiceName không hợp lệ!";
						return returnData;
					}
					if (!Validation.CheckXSSInput(getList_.ServiceName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu ServiceName chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (getList_.ProductsOfServicesID != null)
				{

					if (getList_.ProductsOfServicesID <= 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "Dữ liệu đầu vào ProductsOfServicesID không hợp lệ!";
						return returnData;
					}
					if (await _typeProductsOfServices.GetTypeProductsOfServicesIDByID(getList_.ProductsOfServicesID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = $"Danh sách Service không tồn tại Service có ProductsOfServicesID: {getList_.ProductsOfServicesID}!";
						return returnData;
					}
				}
			
				var parameters = new DynamicParameters();
				parameters.Add("@ServiceID", getList_.ServiceID ?? null);
				parameters.Add("@ServiceName", getList_.ServiceName ?? null);
				parameters.Add("@ProductsOfServicesID", getList_.ProductsOfServicesID ?? null);
				var result = await DbConnection.QueryAsync<ResponseServicess>("GetList_SearchServicess", parameters);
				if (result != null && result.Any()) 
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Lấy danh sách người dùng thành công!";
					returnData.Data = result.ToList();
					_listSevicess = returnData.Data.Select(x => new Servicess
					{
						ServiceID = x.ServiceID ?? 0,
						ProductsOfServicesID = x.ProductsOfServicesID ?? 0,
						ServiceName = x.ServiceName,
						Description = x.Description,
						ServiceImage = x.ServiceImage,
						PriceService = x.PriceService ?? 0,
					}).ToList();
					return returnData;
				}
				else
				{
					returnData.ResponseCode = 0;
					returnData.ResposeMessage = "Không tìm thấy người dùng nào.";
					return returnData;
				}
			}
			catch (Exception ex) 
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

		public async Task<ResponseData> ExportServicessToExcel(ExportExcel filePath)
		{
			var returnData = new ResponseData();
			try
			{
				if (filePath == null || string.IsNullOrWhiteSpace(filePath.filePath))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Đường dẫn file không hợp lệ!";
					return returnData;
				}

				var listServicess = await DbConnection.QueryAsync<ResponseServicess>("GetList_SearchServicess", new DynamicParameters());

				if (listServicess == null || !listServicess.Any())
				{
					returnData.ResponseCode = 0;
					returnData.ResposeMessage = "Không có dữ liệu để xuất!";
					return returnData;
				}

				using (var workBook = new XLWorkbook())
				{
					var workSheet = workBook.AddWorksheet("List_Servicess");
					workSheet.Cell(1, 1).Value = "ServiceID";
					workSheet.Cell(1, 2).Value = "ProductsOfServicesID";
					workSheet.Cell(1, 3).Value = "ServiceName";
					workSheet.Cell(1, 4).Value = "Description";
					workSheet.Cell(1, 5).Value = "ServiceImage";
					workSheet.Cell(1, 6).Value = "PriceService";

					int row = 2;
					foreach (var item in listServicess)
					{
						workSheet.Cell(row, 1).Value = item.ServiceID;
						workSheet.Cell(row, 2).Value = item.ProductsOfServicesID;
						workSheet.Cell(row, 3).Value = item.ServiceName;
						workSheet.Cell(row, 4).Value = item.Description;
						workSheet.Cell(row, 5).Value = item.ServiceImage;
						workSheet.Cell(row, 6).Value = item.PriceService;
						row++;
					}
					string fullPath = filePath.filePath.EndsWith(".xlsx") ? filePath.filePath : filePath.filePath + ".xlsx";
					workBook.SaveAs(fullPath);
				}

				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Xuất file Excel thành công!";
				return returnData;
			}
			catch (Exception ex)
			{
				returnData.ResponseCode = -99;
				returnData.ResposeMessage = ex.Message;
				return returnData;
			}
		}

	}
}
