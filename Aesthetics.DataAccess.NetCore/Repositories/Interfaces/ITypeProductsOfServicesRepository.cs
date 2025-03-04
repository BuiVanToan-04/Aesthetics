using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interface
{
	public interface ITypeProductsOfServicesRepository
	{
		//1.Function thêm TypeProductsOfServices
		Task<ProductsOfServices_Loggin> Insert_TypeProductsOfServices(TypeProductsOfServicesRequest request);

		//2.Function cập nhật TypeProductsOfServices
		Task<ProductsOfServices_Loggin> Update_TypeProductsOfServices(Update_TypeProductsOfServices update_);

		//3.Function xóa TypeProductsOfServices
		Task<ProductsOfServices_Loggin> Delete_TypeProductsOfServices(Delete_TypeProductsOfServices delete_);

		//4.Function get list & Search TypeProductsOfServices
		Task<ProductsOfServicesData> GetList_SearchTypeProductsOfServices(GetList_SearchTypeProductsOfServices getList_Search);

		//5.Funciton get TypeProductsOfServices by TypeProductsOfServicesName
		Task<TypeProductsOfServices> GetTypeByName(string? Name, string? Type);

		//6.Funciton get TypeProductsOfServices by TypeProductsOfServicesID
		Task<TypeProductsOfServices> GetTypeProductsOfServicesIDByID(int? ProductsOfServicesID);

		//7.Function get TypeProductsOfServices by ProductsOfServicesName
		Task<TypeProductsOfServices> GetTypeProductsOfServicesByName(string? ProductsOfServicesName);
	}
}
