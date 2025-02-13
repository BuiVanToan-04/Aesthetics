using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject;
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
		Task<ResponseData> Insert_Servicess(ServicessRequest servicess_);

		//2.Function update Servicess
		Task<ResponseData> Update_Servicess(Update_Servicess update_);

		//3.Function delete Servicess
		Task<ResponseData> Delete_Servicess(Delete_Servicess delete_);

		//4.Function Get list & Search Servicess
		Task<ResponseServicessData> GetList_SearchServicess(GetList_SearchServicess getList_);

		//5.Base Processing Function 64
		Task<string> BaseProcessingFunction64(string? ServicessImage);

		//6.Function get Servicess by ServicesID 
		Task<Servicess> GetServicessByServicesID(int? ServicesID);

		//7.Function get TypeProductsOfServices by TypeProductsOfServicesID and ProductsOfServicesType
		Task<TypeProductsOfServices> GetTypeProductsOfServices(int? TypeProductsOfServicesID, string? ProductsOfServicesType);

		//8.Function get TypeProductsOfServices by ProductsOfServicesType
		Task<TypeProductsOfServices> GetTypeProductsOfServicesByName(string? ProductsOfServicesType);
	}
}
