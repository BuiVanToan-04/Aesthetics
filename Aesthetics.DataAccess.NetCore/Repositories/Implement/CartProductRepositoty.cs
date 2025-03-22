using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DataAccess.NetCore.DBContext;
using Aesthetics.DataAccess.NetCore.Repositories.Interfaces;
using Aesthetics.DTO.NetCore.DataObject.LogginModel;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.ResponseCart_Product;
using BE_102024.DataAces.NetCore.Dapper;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Implement
{
	public class CartProductRepositoty : BaseApplicationService, ICartProductRepository
	{
		private DB_Context _context;
		private IConfiguration _configuration;
		public CartProductRepositoty(DB_Context context, IConfiguration configuration,
			IServiceProvider serviceProvider) : base(serviceProvider)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<ResponseCart_Product_Loggin> Insert_CartProduct(Cart_ProductRequest insert_)
		{
			var returnData = new ResponseCart_Product_Loggin();
			var listCartProduct_Log = new List<CartProducts_Loggin>();
			try
			{
				var carts = await _context.Carts.Where(s => s.CartID == insert_.CartID).FirstOrDefaultAsync();
				if (insert_.CartID <= 0 || carts == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "CartID không hợp lệ || không tồn tại!";
					return returnData;
				}
				var product = await _context.Products.Where(s => s.ProductID == insert_.ProductID
								&& s.DeleteStatus == 1).FirstOrDefaultAsync();
				if (insert_.ProductID <= 0 || product == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "ProductID không hợp lệ || không tồn tại!";
					return returnData;
				}
				if (insert_.Quantity <= 0 || insert_.Quantity > product.Quantity)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Số lượng đặt mua không hợp lệ. Vui lòng nhập lại số lượng!";
					return returnData;
				}
				var createDay = DateTime.Now;
				var newCartProduct = new CartProduct
				{
					CartID = insert_.CartID,
					ProductID = insert_.ProductID,
					Quantity = insert_.Quantity,
					CreateDay = createDay,
				};
				await _context.CartProduct.AddAsync(newCartProduct);
				await _context.SaveChangesAsync();
				listCartProduct_Log.Add(new CartProducts_Loggin
				{
					CartProductID = newCartProduct.CartProductID,
					CartID = newCartProduct.CartID,
					ProductID = newCartProduct.ProductID,
					Quantity = newCartProduct.Quantity,
					CreateDay = newCartProduct.CreateDay,
				});
				returnData.ResponseCode = 1;
				returnData.cartProduct_Loggins = listCartProduct_Log;
				returnData.ResposeMessage = "Insert CartProduct thành công!";
				return returnData;

			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Insert_CartProduct Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseCart_Product_Loggin> Update_CartProduct(Update_Cart_ProductRequest update_)
		{
			var returnData = new ResponseCart_Product_Loggin();
			var listCartProduct_Log = new List<CartProducts_Loggin>();
			try
			{
				var cartProduct_ = await GetCartProductByCartProductID(update_.CartProductID);
				if (update_.CartProductID <= 0 || cartProduct_ == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "CartProductID không hợp lệ || không tồn tại!";
					return returnData;
				}

				var product = await _context.Products.Where(s => s.ProductID == cartProduct_.ProductID
								&& s.DeleteStatus == 1).FirstOrDefaultAsync();

				if (update_.Quantity <= 0 || update_.Quantity > product.Quantity)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "Số lượng đặt mua không hợp lệ. Vui lòng nhập lại số lượng!";
					return returnData;
				}
				cartProduct_.CartProductID = update_.CartProductID;
				cartProduct_.Quantity = update_.Quantity;
				_context.CartProduct.Update(cartProduct_);
				await _context.SaveChangesAsync();
				listCartProduct_Log.Add(new CartProducts_Loggin
				{
					CartProductID = cartProduct_.CartProductID,
					CartID = cartProduct_.CartProductID,
					ProductID = cartProduct_.ProductID,
					Quantity = cartProduct_.Quantity,
					CreateDay = cartProduct_.CreateDay,
				});
				returnData.ResponseCode = 1;
				returnData.ResposeMessage = "Update thành công CartProduct";
				returnData.cartProduct_Loggins = listCartProduct_Log;
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Update_CartProduct Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseCart_Product_Loggin> Delete_CartProduct(Delete_Cart_ProductRequest delete_)
		{
			var returnData = new ResponseCart_Product_Loggin();
			var listCartProduct_Log = new List<CartProducts_Loggin>();
			try
			{
				var resultCartProduct = await GetCartProductByCartProductID(delete_.CartProductID);
				if (resultCartProduct != null)
				{
					_context.CartProduct.Remove(resultCartProduct);
					await _context.SaveChangesAsync();
					listCartProduct_Log.Add(new CartProducts_Loggin
					{
						CartProductID = resultCartProduct.CartProductID,
						CartID = resultCartProduct.CartID,
						ProductID = resultCartProduct.ProductID,
						Quantity = resultCartProduct.Quantity,
						CreateDay = resultCartProduct.CreateDay,
					});
					returnData.ResponseCode = 1;
					returnData.ResposeMessage = "Delete CartProduct thành công!";
					returnData.cartProduct_Loggins = listCartProduct_Log;
					return returnData;
				}
				returnData.ResponseCode = -1;
				returnData.ResposeMessage = "CartProductID không tồn tại!";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Delete_CartProduct Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<ResponseGetList_SearchCart_Product_Loggin> GetList_SearchCartProduct(GetList_SearchCart_ProductRequest getList_SearchCart_)
		{
			var returnData = new ResponseGetList_SearchCart_Product_Loggin();
			var listCartProduct_Log = new List<CartProducts_Loggin>();
			try
			{
				var resultFindCart = await _context.CartProduct.Where(s => s.CartID == getList_SearchCart_.CartID).ToListAsync();
				if (getList_SearchCart_.CartID <= 0 || resultFindCart == null)
				{
					returnData.ResponseCode = -1;
					returnData.ResposeMessage = "CartID không hợp lệ || không tồn tại!";
					return returnData;
				}
				var parameters = new DynamicParameters();
				parameters.Add("@CartID", getList_SearchCart_.CartID);
				var result = await DbConnection.QueryAsync<ResponseCart_Product>("GetList_SearchCart_Product", parameters);
				if (result != null && result.Any()) 
				{
					foreach (var item in result) 
					{
						listCartProduct_Log.Add(new CartProducts_Loggin
						{
							CartProductID = item.CartProductID,
							CartID = getList_SearchCart_.CartID,
							ProductID = item.ProductID,
							Quantity = item.Quantity,
							CreateDay = item.CreateDay,
						});
					}
					returnData.ResponseCode = 1;
					returnData.Data = result.ToList();
					returnData.ResposeMessage = "Lấy thành công danh sách CartProduct!";
					returnData.cartProduct_Loggins = listCartProduct_Log;
					return returnData;
				}
				returnData.ResponseCode = 0;
				returnData.ResposeMessage = "Không tìm thấy CartProduct nào.";
				return returnData;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error in Delete_CartProduct Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<CartProduct> GetCartProductByCartProductID(int cartProductID)
		{
			return await _context.CartProduct.Where(s => s.CartProductID == cartProductID).FirstOrDefaultAsync();
		}
	}
}
