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
	public interface IClinicRepository
	{
		//1.Function insert Clinic
		Task<ResponesClinic_Loggin> Insert_Clinic(ClinicRequest insert_);

		//2.Function update Clinic
		Task<ResponesClinic_Loggin> Update_Clinic(Update_Clinic update_);

		//3.Function delete Clinic
		Task<ResponesClinic_DeleteLoggin> Delete_Clinic(Delete_Clinic delete_);

		//4.Function get list & Search Clinic
		Task<ResponesClinicData> GetList_SearchClinic(GetList_Search getList_);

		//5.Function get Clinic by ClinicName
		Task<Clinic> GetClinicByName(string? name);

		//Function get Clinic by ClinicID
		Task<Clinic> GetClinicByID(int? ClinicID);
	}
}
