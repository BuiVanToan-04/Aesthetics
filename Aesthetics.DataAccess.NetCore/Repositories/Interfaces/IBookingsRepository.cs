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

		//Function get Booking by BookingID
		Task<Booking> GetBookingByID(int? BookingID);

		//Function gen NumberOrder
		Task<(int? NumberOrder, string? Message)> GenerateNumberOrder(DateTime scheduledDate, int? TypeServicessID);

		//Function get TypeProductsOfServicesID by ServicessID
		Task<int?> GetProductsOfServicesIDByServicesID(int? ServicessID);

		//Funciton get ServicessName by ServicessID 
		Task<string?> GetServicessNameByID (int? ServicessID);

		//Function get Clinic by ProductsOfServicesID
		Task<int?> GetClinicByProductsOfServicesID(int? ProductsOfServicesID);

		//Function get ProductsOfServicesID by BooKingID
		Task<int?> GetProductsOfServicesIDByBooKingID(int? BookingID);

		//Function get ServicesID by BooKingID
		Task<int?> GetServicesIDByBookingID(int? BookingID);

		//Function get ScheduledDate by BooKingID
		Task<DateTime?> GetScheduledDateByBookingID(int? BookingID);

		//Function get Booking_Assignment by bookingID 
		Task<Booking_Assignment> GetBooking_AssignmentByBookingID(int? BookingID);
	}
}
