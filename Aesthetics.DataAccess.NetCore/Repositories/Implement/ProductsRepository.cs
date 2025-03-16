using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interface;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class ProductsRepository : BaseApplicationService, IProductsRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		private static List<Products> _listProducts;
		private ISupplierRepository _supplierRepository;
		public ProductsRepository(DB_Context context, IConfiguration configuration, 
			IServiceProvider serviceProvider, ISupplierRepository supplierRepository) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
			_listProducts = new List<Products>();
			_supplierRepository = supplierRepository;
		}
		public async Task<string> BaseProcessingFunction64(string? ProductsImage)
		{
			try
			{
				var path = "FilesImages/Products";

				if (!System.IO.Directory.Exists(path))
				{
					System.IO.Directory.CreateDirectory(path);
				}
				string imageName = Guid.NewGuid().ToString() + ".png";
				var imgPath = Path.Combine(path, imageName);

				if (ProductsImage.Contains("data:image"))
				{
					ProductsImage = ProductsImage.Substring(ProductsImage.LastIndexOf(',') + 1);
				}

				byte[] imageBytes = Convert.FromBase64String(ProductsImage);
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

		public async Task<Products> GetProductsByProductID(int? ProductID)
		{
			return await _context.Products.Where(s => s.ProductID == ProductID && s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<TypeProductsOfServices> GetProductOfServicesByID(int? ProductsOfServicesID)
		{
			return await _context.TypeProductsOfServices.Where(s => s.ProductsOfServicesID == ProductsOfServicesID
				&& s.ProductsOfServicesType == "Products"
				&& s.DeleteStatus == 1).FirstOrDefaultAsync();
		}

		public async Task<ResponseProducts_Loggin> Insert_Products(ProductRequest products_)
		{
			var returnData = new ResponseProducts_Loggin();
			var product_Loggins = new List<Products_Loggin>();
			try
			{
				if (products_.ProductsOfServicesID <= 0 
					|| await GetProductOfServicesByID(products_.ProductsOfServicesID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ProductsOfServicesID không hợp lệ || không tồn tại!";
					return returnData;
				}

				if (products_.SupplierID <= 0 
					|| await _supplierRepository.GetSupplierBySupplierID(products_.SupplierID) == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "SupplierID không hợp lệ || không tồn tại!";
					return returnData;
				}

				if (!Validation.CheckString(products_.ProductName) || !Validation.CheckXSSInput(products_.ProductName))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ProductName không hợp lệ || chứa kí tự không hợp lệ!";
					return returnData;
				}

				if (!Validation.CheckString(products_.ProductDescription) || !Validation.CheckXSSInput(products_.ProductDescription))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ProductDescription không hợp lệ || chứa kí tự không hợp lệ!";
					return returnData;
				}

				if (products_.SellingPrice < 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "SellingPrice không hợp lệ!";
					return returnData;
				}

				if (products_.Quantity < 0)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "SellingPrice không hợp lệ!";
					return returnData;
				}

				if (!Validation.CheckXSSInput(products_.ProductImages) || !Validation.CheckString(products_.ProductImages))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ProductImages không hợp lệ || chứa kí tự không hợp lệ!";
					return returnData;
				}

				var imagePathProducts = await BaseProcessingFunction64(products_.ProductImages);
				var parametters = new DynamicParameters();
				parametters.Add("@ProductsOfServicesID", products_.ProductsOfServicesID);
				parametters.Add("@SupplierID", products_.SupplierID);
				parametters.Add("@ProductName", products_.ProductName);
				parametters.Add("@ProductDescription", products_.ProductDescription);
				parametters.Add("@SellingPrice", products_.SellingPrice);
				parametters.Add("@Quantity", products_.Quantity);
				parametters.Add("@ProductImages", imagePathProducts);
				parametters.Add("@ProductID", dbType: DbType.Int32, direction: ParameterDirection.Output); 
				await DbConnection.ExecuteAsync("Insert_Products", parametters);
				var newProductID = parametters.Get<int?>("@ProductID") ?? 0;
				product_Loggins.Add(new Products_Loggin
				{
					ProductID = newProductID,
					ProductsOfServicesID = products_.ProductsOfServicesID,
					SupplierID = products_.SupplierID,
					ProductName = products_.ProductName,
					ProductDescription = products_.ProductDescription,
					SellingPrice = products_.SellingPrice,
					Quantity = products_.Quantity,
					ProductImages = imagePathProducts,
					DeleteStatus = 1
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Thêm mới sản phẩm thành công!";
				returnData.products_Loggins = product_Loggins;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Insert_Products Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseProducts_Loggin> Update_Products(Update_Products update_)
		{
			var returnData = new ResponseProducts_Loggin();
			var product_Loggins = new List<Products_Loggin>();
			try
			{
				var products = await _context.Products
						.Where(s => s.ProductID == update_.ProductID && s.DeleteStatus == 1).FirstOrDefaultAsync();
				if (update_.ProductID <= 0 || products == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ProductID không hợp lệ || không tồn tại!";
					return returnData;
				}
				if (update_.ProductName != null)
				{
					if (!Validation.CheckString(update_.ProductName) || !Validation.CheckXSSInput(update_.ProductName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductName không hợp lệ || chứa kí tự không hợp lệ!";
						return returnData;
					}
					products.ProductName = update_.ProductName;
				}
				if (update_.ProductsOfServicesID != null)
				{
					if (await GetProductOfServicesByID(update_.ProductsOfServicesID) == null 
						|| update_.ProductsOfServicesID <= 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductsOfServicesID không hợp lệ || không tồn tại!";
						return returnData;
					}
					products.ProductsOfServicesID = update_.ProductsOfServicesID;
				}
				if (update_.SupplierID != null)
				{
					if (update_.SupplierID <= 0 ||  await _supplierRepository.GetSupplierBySupplierID(update_.SupplierID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "SupplierID không hợp lệ || không tồn tại!";
						return returnData;
					}
					products.SupplierID = update_.SupplierID;
				}
				if (update_.ProductDescription != null)
				{
					if (!Validation.CheckString(update_.ProductDescription) || !Validation.CheckXSSInput(update_.ProductDescription))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductDescription không hợp lệ || chứa kí tự không hợp lệ!";
						return returnData;
					}
					products.ProductDescription = update_.ProductDescription;
				}
				if (update_.SellingPrice != null)
				{
					if (update_.SellingPrice < 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "SellingPrice không hợp lệ!";
						return returnData;
					}
					products.SellingPrice = update_.SellingPrice;
				}
				if (update_.Quantity != null)
				{
					if (update_.Quantity < 0)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "SellingPrice không hợp lệ!";
						return returnData;
					}
					products.Quantity = update_.Quantity;
				}
				if (update_.ProductImages != null)
				{
					if (!Validation.CheckXSSInput(update_.ProductImages) || !Validation.CheckString(update_.ProductImages))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductImages không hợp lệ || chứa kí tự không hợp lệ!";
						return returnData;
					}
					var imagePathProducts = await BaseProcessingFunction64(update_.ProductImages);
					products.ProductImages = imagePathProducts;
				}
				_context.Products.Update(products);
				await _context.SaveChangesAsync();
				product_Loggins.Add(new Products_Loggin
				{
					ProductID = products.ProductID,
					ProductsOfServicesID = products.ProductsOfServicesID,
					SupplierID = products.SupplierID,
					ProductName = products.ProductName,
					ProductDescription = products.ProductDescription,
					SellingPrice = products.SellingPrice,
					Quantity = products.Quantity,
					ProductImages = products.ProductImages,
					DeleteStatus = 1
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Update Product thành công!";
				returnData.products_Loggins = product_Loggins;
				return returnData;
			}
			catch(Exception ex) 
			{
				throw new Exception($"Error Update_Products Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseProducts_LogginDelete> Delete_Products(Delete_Products delete_)
		{
			var returnData = new ResponseProducts_LogginDelete();
			var product_Loggins = new List<Products_Loggin>();
			var comment_Loggins = new List<Comment_Loggin>();
			var cartProducts_Loggins = new List<CartProducts_Loggin>();
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var products = await _context.Products
					.Include(a => a.Comments)
					.Include(b => b.CartProducts)
					.AsSplitQuery()
					.Where(s => s.ProductID == delete_.ProductID && s.DeleteStatus == 1).FirstOrDefaultAsync();
				if (products != null)
				{
					//1.Xóa Products
					products.DeleteStatus = 0;
					product_Loggins.Add(new Products_Loggin
					{
						ProductID = products.ProductID,
						ProductsOfServicesID = products.ProductsOfServicesID,
						SupplierID = products.SupplierID,
						ProductName = products.ProductName,
						ProductDescription = products.ProductDescription,
						SellingPrice = products.SellingPrice,
						Quantity = products.Quantity,
						ProductImages = products.ProductImages,
						DeleteStatus = 0
					});

					//2.Xóa Comment
					var comments = products.Comments
						.Where(s => s.ProductID == products.ProductID).ToList();
					if (comments != null && comments.Any())
					{
						foreach (var item in comments)
						{
							_context.Comments.Remove(item);
							//Lưu log comment
							comment_Loggins.Add(new Comment_Loggin
							{
								CommentID = item.CommentID,
								ProductID = item.ProductID,
								ServiceID = item.ServiceID,
								UserID = item.UserID,
								Comment_Content = item.Comment_Content,
								CreationDate = item.CreationDate,
							});
						}
					}

					//3.Xóa CartProducts
					var cartProducts = products.CartProducts
						.Where(s => s.ProductID == products.ProductID).ToList();
					if (cartProducts != null && cartProducts.Any())
					{
						foreach (var item in cartProducts)
						{
							_context.CartProduct.Remove(item);
							//Lưu log cartProducts
							cartProducts_Loggins.Add(new CartProducts_Loggin
							{
								CartProductID = item.CartProductID,
								CartID = item.CartID,
								ProductID = item.ProductID,
								Quantity = item.Quantity,
								CreateDay = item.CreateDay
							});
						}
					}

					await transaction.CommitAsync();
					await _context.SaveChangesAsync();
					
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Delete Product thành công!";
					returnData.products_Loggins = product_Loggins;
					returnData.comment_Loggins = comment_Loggins;
					returnData.cartProducts_Loggins = cartProducts_Loggins;
					return returnData;
				}

				returnData.ResponseCode = -1;
				returnData.ResposeMessage = "ProductID không hợp lệ || không tồn tại!";
				return returnData;
			}
			catch (Exception ex) 
			{
				await transaction.RollbackAsync();
				throw new Exception($"Error Delete_Products Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseProducts_LogginGetList> GetList_SearchProducts(GetList_SearchProducts getList_)
		{
			var returnData = new ResponseProducts_LogginGetList();
			try
			{
				if (getList_.ProductID != null)
				{
					if (getList_.ProductID <= 0 || await GetProductsByProductID(getList_.ProductID) == null)
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductID không hợp lệ || không tồn tại!";
						return returnData;
					}
				}
				if (getList_.ProductsOfServicesName != null)
				{
					if (!Validation.CheckString(getList_.ProductsOfServicesName) || !Validation.CheckXSSInput(getList_.ProductsOfServicesName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductsOfServicesName không hợp lệ || Chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (getList_.SupplierName != null)
				{
					if (!Validation.CheckString(getList_.SupplierName) || !Validation.CheckXSSInput(getList_.SupplierName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "SupplierName không hợp lệ || Chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				if (getList_.ProductName != null)
				{
					if (!Validation.CheckString(getList_.ProductName) || !Validation.CheckXSSInput(getList_.ProductName))
					{
						returnData.ResponseCode = -1;
						returnData.ResposeMessage = "ProductName không hợp lệ || Chứa kí tự không hợp lệ!";
						return returnData;
					}
				}
				var parametters = new DynamicParameters();
				parametters.Add("@ProductID", getList_.ProductID ?? null);
				parametters.Add("@ProductName", getList_.ProductName ?? null);
				parametters.Add("@ProductsOfServicesName", getList_.ProductsOfServicesName ?? null);
				parametters.Add("@SupplierName", getList_.SupplierName ?? null);

				var result = await DbConnection.QueryAsync<ResponseGetListProducts>("GetList_SearchProduct", parametters);
				if (result != null && result.Any())
				{
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Get list Products thành công!";
					returnData.Data = result.ToList();
					return returnData;
				}
				returnData.ResponseCode = 0;
				returnData.ResposeMessage = "Không tìm thấy Products nào.";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error GetList_SearchProducts Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseData> ExportProductsToExcel(ExportExcel filePath)
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

				// Kiểm tra xem file có phải định dạng Excel không
				string extension = Path.GetExtension(filePath.filePath);
				if (string.IsNullOrEmpty(extension) || (extension.ToLower() != ".xlsx" && extension.ToLower() != ".xls"))
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "File không đúng định dạng Excel (.xlsx hoặc .xls)!";
					return returnData;
				}

				var listProduct = await DbConnection.QueryAsync<ResponseGetListProducts>("GetList_SearchProduct", new DynamicParameters());

				if (listProduct != null && listProduct.Any())
				{
					using (var workBook = new XLWorkbook())
					{
						var workSheet = workBook.AddWorksheet("List_Products");

						workSheet.Cell(1, 1).Value = "ProductID";
						workSheet.Cell(1, 2).Value = "ProductName";
						workSheet.Cell(1, 3).Value = "SellingPrice";
						workSheet.Cell(1, 4).Value = "Quantity";
						workSheet.Cell(1, 5).Value = "ProductImages";
						workSheet.Cell(1, 6).Value = "ProductDescription";
						workSheet.Cell(1, 7).Value = "ProductsOfServicesName";
						workSheet.Cell(1, 8).Value = "SupplierName";
						workSheet.Cell(1, 9).Value = "DeleteStatus";

						int row = 2;
						foreach (var item in listProduct)
						{
							workSheet.Cell(row, 1).Value = item.ProductID;
							workSheet.Cell(row, 2).Value = item.ProductName;
							workSheet.Cell(row, 3).Value = item.SellingPrice;
							workSheet.Cell(row, 4).Value = item.Quantity;
							workSheet.Cell(row, 5).Value = item.ProductImages;
							workSheet.Cell(row, 6).Value = item.ProductDescription;
							workSheet.Cell(row, 7).Value = item.ProductsOfServicesName;
							workSheet.Cell(row, 8).Value = item.SupplierName;
							workSheet.Cell(row, 9).Value = item.DeleteStatus;
							row++;
						}

						workBook.SaveAs(filePath.filePath);

						returnData.ResponseCode = 1;
						returnData.ResposeMessage = "Xuất file Excel thành công!";
						return returnData;
					}
				}

				returnData.ResponseCode = 0;
				returnData.ResposeMessage = "Không có dữ liệu để xuất!";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error ExportProductsToExcel Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}
	}
}
