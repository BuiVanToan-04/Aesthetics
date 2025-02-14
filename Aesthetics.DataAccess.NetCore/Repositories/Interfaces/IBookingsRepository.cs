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
		//1.Function add Booking
		Task<ResponseData> Insert_Booking(BookingRequest insert_);

		//2.Function update Booking
		Task<ResponseData> Update_Booking(Update_Booking update_);

		//3.Function delete Booking
		Task<ResponseData> Delete_Booking(Delete_Booking delete_);

		//4.Function get list & search Booking
		Task<ResponseData> GetList_SearchBooking(GetList_SearchBooking getList_);

		//5.Function get Booking by BookingID
		Task<Booking> GetBookingByID(int? BookingID);

		//6.Function gen NumberOrder
		int GenerateNumberOrder();
	}
}
