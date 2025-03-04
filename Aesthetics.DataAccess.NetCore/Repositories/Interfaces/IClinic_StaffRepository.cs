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
    public interface IClinic_StaffRepository
    {
		//1. Insert Clinic_Staff
		Task<Response_ClinicStaff_Loggin> Insert_Clinic_Staff(Clinic_StaffRequest insert_);

		//2. Update Clinic_Staff
		Task<Response_ClinicStaff_Loggin> Update_Clinic_Staff(Clinic_StaffUpdate update_);

		//3. Delete Clinic_Staff
		Task<Response_ClinicStaff_Loggin> Delete_Clinic_Staff(Clinic_StaffDelete delete_);

		//4. GetList_Clinic_Staff
		Task<Response_ClinicStaff_Loggin> GetList_Clinic_Staff(Clinic_StaffGetList getList_);

		//5. Get Clinic_Staff by ClinicStaffID
		Task<Clinic_Staff> GetClinic_StaffByID(int? ClinicStaffID);
	}
}
