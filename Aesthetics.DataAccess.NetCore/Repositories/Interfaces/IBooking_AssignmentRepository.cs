using Aesthetics.DataAccess.NetCore.CheckConditions.Response;
using Aesthetics.DTO.NetCore.DataObject;
using Aesthetics.DTO.NetCore.RequestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.DataAccess.NetCore.Repositories.Interfaces
{
	public interface IBooking_AssignmentRepository
	{
		//Funciton Delete Booking_Assignment
		Task<ResponseData> Delete_Booking_Assignment();

		//Funciton get list & search Booking_Assignment
		Task<ResponseData> GetList_SearchBooking_Assignment();
	}
}
