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
	public interface IBookingsRepository
	{
		//1.Function add Booking
		Task<ResponseBooking_Ser_Ass> Insert_Booking(BookingRequest insert_);

		//3.Function delete Booking
		Task<ResponseBooking_Ser_Ass> Delete_Booking(Delete_Booking delete_);

		//4.Function get list & search Booking
		Task<ResponseBookingData> GetList_SearchBooking(GetList_SearchBooking getList_);

		//5.Function get list & search Booking_Assignment
		Task<ResponseBooking_AssignmentData> GetList_SearchBooking_Assignment(GetList_SearchBooking_Assignment getList_);

		//6.Function get list & search Booking_Servicess
		Task<ResponseBooking_ServicesData> GetList_SearchBooking_Services(GetList_SearchBooking_Services getList_);

		//7. Insert booking_Services & booking_Assignment
		Task<ResponseBooking_Ser_Ass> Insert_BookingSer_Assi(Insert_Booking_Services insert_);

		//8. Delete booking_Services & booking_Assignment
		Task<ResponseBooking_Ser_Ass> Delete_BookingSer_Assi(Delete_Booking_Services insert_);

		//8.Function gen NumberOrder
		Task<(int? NumberOrder, string? Message)> GenerateNumberOrder(DateTime assignedDate, int? ProductsOfServicesID);

		//9.Funciton get Servicess by ServicessID 
		Task<Servicess> GetServicessByServicessID (int servicessID);

		//10.Function get ClinicID by ProductsOfServicesID
		Task<int> GetClinicIDByProductsOfServicesID(int? ProductsOfServicesID);

		//11.Function get Booking by BookingID 
		Task<Booking> GetBooKingByBookingID(int? BookingID);

		//12.Funcion get Booking_Assignment by Booking_AssignmentID 
		Task<BookingAssignment> GetBooking_AssignmentByID(int? AssignmentID);

		//13.Funciton get Servicess by ServicessName
		Task<Servicess> GetServicessByServicessName(string? servicessName);

		//14. Get ProductOfServicesID by ServisesID
		Task<int> GetProductOfServicesIDByServisesID(int servicesID);

		//15. Get UserName by UserID
		Task<string> GetUserNameByUserID(int userId);

		//16. Get ServicesName by ServicesID
		Task<string> GetServicesNamebyServicesID(int servicesID);

	}
}
