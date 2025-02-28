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
	public interface IBookingsRepository
	{
		//1.Function add Booking
		Task<ResponseBooking_Ser_Ass> Insert_Booking(BookingRequest insert_);

		//2.Function update Booking
		Task<ResponseBookingUpdate_Ser_Ass> Update_Booking(Update_Booking update_);

		//3.Function delete Booking
		Task<ResponseBooking_Ser_Ass> Delete_Booking(Delete_Booking delete_);

		//4.Function get list & search Booking
		Task<ResponseBookingData> GetList_SearchBooking(GetList_SearchBooking getList_);

		//5.Function get list & search Booking_Assignment
		Task<ResponseBooking_AssignmentData> GetList_SearchBooking_Assignment(GetList_SearchBooking_Assignment getList_);

		//6.Function gen NumberOrder
		Task<(int? NumberOrder, string? Message)> GenerateNumberOrder(DateTime assignedDate, int? ProductsOfServicesID);

		//7.Funciton get Servicess by ServicessID 
		Task<Servicess> GetServicessByServicessID (int servicessID);

		//8.Function get ClinicID by ProductsOfServicesID
		Task<int> GetClinicIDByProductsOfServicesID(int? ProductsOfServicesID);

		//9.Function get Booking by BookingID 
		Task<Booking> GetBooKingByBookingID(int? BookingID);

		//10.Funcion get Booking_Assignment by Booking_AssignmentID 
		Task<Booking_Assignment> GetBooking_AssignmentByID(int? AssignmentID);

		//11.Funciton get Servicess by ServicessName
		Task<Servicess> GetServicessByServicessName(string? servicessName);
	}
}
