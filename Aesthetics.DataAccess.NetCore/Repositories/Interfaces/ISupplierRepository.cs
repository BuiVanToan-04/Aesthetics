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
	public interface ISupplierRepository
	{
		//1.Function kiểm tra tồn tại của SupplierName
		Task<Supplier> GetSupplierBySupplierName(string? supplierName);

		//2.Function kiểm tra tồn tại của Supplier qua SupplierID
		Task<Supplier> GetSupplierBySupplierID(int? supplierID);

		//3.Function thêm Supplier
		Task<ResponseData> Insert_Supplier(SupplierRequest supplier);

		//4.Function cập nhật Supplier
		Task<ResponseData> Update_Supplier(Update_Supplier supplier);

		//5.Function xóa Supplier
		Task<ResponseData> Delete_Supplier(Delete_Supplier supplier);

		//6.Get list Supplier & Search Supplier by SupplierName or SupplierID
		Task<ResponseSupplierData> GetList_SearchSupplier(GetList_SearchSupplier _searchSupplier);
	}
}
