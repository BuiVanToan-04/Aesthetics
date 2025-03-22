using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.ResponseCart_Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interfaces
{
    public interface ICartProductRepository
    {
        //1.Insert CartProduct
        Task<ResponseCart_Product_Loggin> Insert_CartProduct(Cart_ProductRequest insert_);

		//2. Update CartProduct
		Task<ResponseCart_Product_Loggin> Update_CartProduct(Update_Cart_ProductRequest update_);

		//3.Delete CartProduct
		Task<ResponseCart_Product_Loggin> Delete_CartProduct(Delete_Cart_ProductRequest delete_);

		//4. Get list & Search CartProduct by CartProductID
		Task<ResponseGetList_SearchCart_Product_Loggin> GetList_SearchCartProduct(GetList_SearchCart_ProductRequest getList_SearchCart_);

		//5. Get CartProduct by CartProductID
		Task<CartProduct> GetCartProductByCartProductID(int  cartProductID);

	}
}
