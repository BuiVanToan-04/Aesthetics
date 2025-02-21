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
	public interface IBookingsRepository
	{
		//Function add Booking
		Task<ResponseData> Insert_Booking(BookingRequest insert_);

		//Function update Booking
		Task<ResponseData> Update_Booking(Update_Booking update_);

		//Function delete Booking
		Task<ResponseData> Delete_Booking(Delete_Booking delete_);

		//Function get list & search Booking
		Task<ResponseData> GetList_SearchBooking(GetList_SearchBooking getList_);

		//Function gen NumberOrder
		Task<(int? NumberOrder, string? Message)> GenerateNumberOrder(DateTime assignedDate, int? ProductsOfServicesID);

		//Funciton get Servicess by ServicessID 
		Task<Servicess> GetServicessByServicessID (int servicessID);

		//Function get ClinicID by ProductsOfServicesID
		Task<int> GetClinicIDByProductsOfServicesID(int? ProductsOfServicesID);

		//Function get Booking by BookingID 
		Task<Booking> GetBooKingByBookingID(int? BookingID);
	}
}
