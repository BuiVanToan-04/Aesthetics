using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject.Model;
using Aesthetics.DTO.NetCore.RequestData;
using Aesthetics.DTO.NetCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interfaces
{
	public interface IServicessRepository
	{
		//1.Function insert Servicess
		Task<ResponseServicess_Loggin> Insert_Servicess(ServicessRequest servicess_);

		//2.Function update Servicess
		Task<ResponseServicess_Loggin> Update_Servicess(Update_Servicess update_);

		//3.Function delete Servicess
		Task<ResponseServicess_LogginDelete> Delete_Servicess(Delete_Servicess delete_);

		//4.Function Get list & Search Servicess
		Task<ResponseServicess_Loggin> GetList_SearchServicess(GetList_SearchServicess getList_);

		//5.Base Processing Function 64
		Task<string> BaseProcessingFunction64(string? ServicessImage);

		//6.Function get Servicess by ServicesID 
		Task<Servicess> GetServicessByServicesID(int? ServicesID);

		//7.Function Export Servicess list to Excel
		Task<ResponseData> ExportServicessToExcel(ExportExcel filePath);

		//8. Function get ProductOfServices by ProductsOfServicesID
		Task<TypeProductsOfServices> GetProductOfServicesByID(int? ProductsOfServicesID);
	}
}
