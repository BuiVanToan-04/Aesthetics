using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.ResponseVouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interfaces
{
    public interface IVouchersRepository
    {
        //1.Insert Vouchers
        Task<ResponseVouchers_Loggin> Insert_Vouchers(VouchersRequest insert_);

		//2.Update Vouchers
		Task<ResponseVouchers_Loggin> Update_Vouchers(Update_Vouchers update_);

		//3.Delete Vouchers
		Task<ResponseVouchers_Loggin> Delete_Vouchers(Delete_Vouchers delete_);

		//4..Get list & Search Vouchers
		Task<ResponseVouchers_Loggin> GetList_SearchVouchers(GetList_SearchVouchers getList_);

		//5.Base Processing Function 64
		Task<string> BaseProcessingFunction64(string? vouchersImage);

		//6. Tạo code & Ktra trùng
		Task<string> GenCodeUnique();
	}
}
